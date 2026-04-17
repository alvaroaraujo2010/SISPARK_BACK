using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sispark_api.Contracts.Parking;
using sispark_api.Domain.Entities;
using sispark_api.Infrastructure.Data;
using System.Security.Claims;

namespace sispark_api.Controllers;

[ApiController]
[Route("api/parking")]
public class ParkingController(SisparkDbContext dbContext) : ControllerBase
{
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ActiveVehicleResponse>>> GetActiveVehicles(CancellationToken cancellationToken)
    {
        var vehicles = await dbContext.RegistrosParqueo
            .AsNoTracking()
            .Include(item => item.Vehiculo)
            .Include(item => item.TipoServicio)
            .Where(item => item.FechaSalida == null)
            .OrderByDescending(item => item.FechaIngreso)
            .Select(item => new ActiveVehicleResponse(
                item.IdRegistro,
                item.Vehiculo != null ? item.Vehiculo.Placa : string.Empty,
                item.FechaIngreso,
                item.FechaSalida,
                item.ValorFinal,
                item.TipoServicio != null ? item.TipoServicio.Nombre : "Sin tipo"))
            .ToListAsync(cancellationToken);

        return Ok(vehicles);
    }

    [Authorize]
    [HttpPost("entry-exit")]
    public async Task<IActionResult> RegisterEntryOrExit(ParkingMovementRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Plate))
        {
            return BadRequest(new { message = "La placa es obligatoria." });
        }

        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "No fue posible identificar el usuario autenticado." });
        }

        var plate = request.Plate.Trim().ToUpperInvariant();
        var vehicle = await dbContext.Vehiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Placa == plate, cancellationToken);

        if (vehicle is null)
        {
            return NotFound(new { message = "La placa no está registrada en la base de vehículos." });
        }

        var activeRecord = await dbContext.RegistrosParqueo
            .FirstOrDefaultAsync(item => item.IdVehiculo == vehicle.IdVehiculo && item.FechaSalida == null, cancellationToken);

        if (activeRecord is null)
        {
            var service = await dbContext.TiposServicio
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Nombre == "Hora", cancellationToken);

            if (service is null)
            {
                return BadRequest(new { message = "No existe el tipo de servicio por hora." });
            }

            var tarifa = await dbContext.Tarifas
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.IdTipoServicio == service.IdTipoServicio
                        && item.IdTipoVehiculo == vehicle.IdTipoVehiculo
                        && item.FechaFinVigencia == null,
                    cancellationToken);

            if (tarifa is null)
            {
                return BadRequest(new { message = "No existe una tarifa activa para este vehículo." });
            }

            var openedStateId = await GetStateIdAsync("registro", "Abierto", cancellationToken);

            var newRecord = new RegistroParqueo
            {
                IdVehiculo = vehicle.IdVehiculo,
                IdTipoServicio = service.IdTipoServicio,
                IdTarifa = tarifa.IdTarifa,
                FechaIngreso = DateTime.Now,
                IdUsuarioIngreso = userId.Value,
                ValorCalculado = 0,
                ValorFinal = 0,
                IdEstado = openedStateId
            };

            dbContext.RegistrosParqueo.Add(newRecord);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                success = true,
                action = "entry",
                message = "Vehículo registrado correctamente."
            });
        }

        var exitDate = DateTime.Now;
        var elapsedMinutes = Math.Max(1, (int)Math.Ceiling((exitDate - activeRecord.FechaIngreso).TotalMinutes));
        var totalToPay = await CalculateParkingTotalAsync(activeRecord.IdTarifa, elapsedMinutes, cancellationToken);
        var closedStateId = await GetStateIdAsync("registro", "Cerrado", cancellationToken);

        activeRecord.FechaSalida = exitDate;
        activeRecord.MinutosConsumidos = elapsedMinutes;
        activeRecord.ValorCalculado = totalToPay;
        activeRecord.ValorFinal = totalToPay;
        activeRecord.IdUsuarioSalida = userId.Value;
        activeRecord.IdEstado = closedStateId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            success = true,
            action = "exit",
            message = "Salida registrada correctamente.",
            totalToPay
        });
    }

    [HttpPost("registrations/preview")]
    public IActionResult PreviewRegistration(ParkingRegistrationPreviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Plate) || string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { message = "Nombre completo y placa son obligatorios." });
        }

        return Ok(new
        {
            success = true,
            message = "Vista previa recibida correctamente.",
            request.FullName,
            Plate = request.Plate.ToUpperInvariant(),
            request.PaymentType
        });
    }

    [Authorize]
    [HttpGet("monthly-vehicles")]
    public async Task<IActionResult> GetMonthlyVehicles(CancellationToken cancellationToken)
    {
        var vehicles = await dbContext.Mensualidades
            .AsNoTracking()
            .Include(item => item.Vehiculo)
            .Include(item => item.Cliente)
            .OrderBy(item => item.FechaFin)
            .Select(item => new
            {
                item.IdMensualidad,
                Cliente = item.Cliente != null ? item.Cliente.NombreCompleto : string.Empty,
                Placa = item.Vehiculo != null ? item.Vehiculo.Placa : string.Empty,
                item.FechaInicio,
                item.FechaFin,
                item.Valor,
                item.IdEstado
            })
            .ToListAsync(cancellationToken);

        return Ok(vehicles);
    }

    private async Task<int> GetStateIdAsync(string modulo, string nombre, CancellationToken cancellationToken)
    {
        var stateId = await dbContext.Estados
            .AsNoTracking()
            .Where(item => item.Modulo == modulo && item.Nombre == nombre)
            .Select(item => item.IdEstado)
            .FirstOrDefaultAsync(cancellationToken);

        if (stateId == 0)
        {
            throw new InvalidOperationException($"No se encontró el estado '{modulo}:{nombre}'.");
        }

        return stateId;
    }

    private async Task<decimal> CalculateParkingTotalAsync(int tarifaId, int elapsedMinutes, CancellationToken cancellationToken)
    {
        var tarifa = await dbContext.Tarifas
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.IdTarifa == tarifaId, cancellationToken);

        if (tarifa is null)
        {
            throw new InvalidOperationException("No se encontró la tarifa asociada al registro.");
        }

        var fractionMinutes = tarifa.FraccionMinutos.GetValueOrDefault(60);
        var units = Math.Max(1, (int)Math.Ceiling(elapsedMinutes / (decimal)fractionMinutes));
        return units * tarifa.Valor;
    }

    private int? GetAuthenticatedUserId()
    {
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.TryParse(rawUserId, out var userId) ? userId : null;
    }
}
