using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sispark_api.Contracts.Admin;
using sispark_api.Domain.Entities;
using sispark_api.Infrastructure.Data;
using System.Security.Claims;

namespace sispark_api.Controllers;

[ApiController]
[Authorize]
[Route("api/admin")]
public class AdminController(SisparkDbContext dbContext) : ControllerBase
{
    [HttpGet("dashboard-summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var weekLimit = DateOnly.FromDateTime(today.AddDays(7));

        var activeVehicles = await dbContext.RegistrosParqueo
            .AsNoTracking()
            .CountAsync(item => item.FechaSalida == null, cancellationToken);

        var availableStateId = await GetStateIdAsync("celda", "Disponible", cancellationToken);
        var availableSpots = await dbContext.Celdas
            .AsNoTracking()
            .CountAsync(item => item.IdEstado == availableStateId, cancellationToken);

        var dailyRevenue = await dbContext.RegistrosParqueo
            .AsNoTracking()
            .Where(item => item.FechaSalida != null && item.FechaSalida >= today)
            .SumAsync(item => (decimal?)item.ValorFinal, cancellationToken) ?? 0;

        var vigenteStateId = await GetStateIdAsync("mensualidad", "Vigente", cancellationToken);
        var monthlyDueSoon = await dbContext.Mensualidades
            .AsNoTracking()
            .CountAsync(
                item => item.IdEstado == vigenteStateId
                    && item.FechaFin >= DateOnly.FromDateTime(today)
                    && item.FechaFin <= weekLimit,
                cancellationToken);

        return Ok(new DashboardSummaryResponse(
            activeVehicles,
            availableSpots,
            dailyRevenue,
            monthlyDueSoon));
    }

    [HttpGet("vehicle-types")]
    public async Task<ActionResult<IEnumerable<VehicleTypeResponse>>> GetVehicleTypes(CancellationToken cancellationToken)
    {
        var vehicleTypes = await dbContext.TiposVehiculo
            .AsNoTracking()
            .OrderBy(item => item.Nombre)
            .Select(item => new VehicleTypeResponse(item.IdTipoVehiculo, item.Nombre))
            .ToListAsync(cancellationToken);

        return Ok(vehicleTypes);
    }

    [HttpPost("vehicle-registrations")]
    public async Task<IActionResult> RegisterVehicle(VehicleRegistrationRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)
            || string.IsNullOrWhiteSpace(request.Plate)
            || request.VehicleTypeId <= 0)
        {
            return BadRequest(new { message = "Los datos del cliente, placa y tipo de vehiculo son obligatorios." });
        }

        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "No fue posible identificar el usuario autenticado." });
        }

        var plate = request.Plate.Trim().ToUpperInvariant();
        var existingVehicle = await dbContext.Vehiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Placa == plate, cancellationToken);

        if (existingVehicle is not null)
        {
            return Conflict(new { message = "La placa ya existe en el sistema." });
        }

        var activeStateId = await GetStateIdAsync("general", "Activo", cancellationToken);
        var existingClient = await dbContext.Clientes
            .FirstOrDefaultAsync(
                item => item.TipoDocumento == request.IdentificationType
                    && item.NumeroDocumento == request.IdentificationNumber,
                cancellationToken);

        if (existingClient is null)
        {
            existingClient = new Cliente
            {
                TipoDocumento = request.IdentificationType.Trim().ToUpperInvariant(),
                NumeroDocumento = request.IdentificationNumber.Trim(),
                NombreCompleto = request.FullName.Trim(),
                Correo = request.Email.Trim(),
                Telefono = request.Phone.Trim(),
                Direccion = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                IdEstado = activeStateId,
                FechaCreacion = DateTime.Now,
                IdUsuarioCreacion = userId.Value
            };

            dbContext.Clientes.Add(existingClient);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            existingClient.NombreCompleto = request.FullName.Trim();
            existingClient.Correo = request.Email.Trim();
            existingClient.Telefono = request.Phone.Trim();
            existingClient.Direccion = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
            existingClient.FechaActualizacion = DateTime.Now;
            existingClient.IdUsuarioActualizacion = userId.Value;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var vehicle = new Vehiculo
        {
            Placa = plate,
            Marca = request.Brand.Trim(),
            Modelo = request.VehicleModel.Trim(),
            Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color.Trim(),
            IdTipoVehiculo = request.VehicleTypeId,
            IdCliente = existingClient.IdCliente,
            Observaciones = string.IsNullOrWhiteSpace(request.Comments)
                ? $"Tipo de pago sugerido: {request.PaymentType}"
                : $"{request.Comments.Trim()} | Tipo de pago sugerido: {request.PaymentType}",
            IdEstado = activeStateId,
            FechaCreacion = DateTime.Now,
            IdUsuarioCreacion = userId.Value
        };

        dbContext.Vehiculos.Add(vehicle);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            success = true,
            message = "Cliente y vehiculo registrados correctamente.",
            clientId = existingClient.IdCliente,
            vehicleId = vehicle.IdVehiculo,
            userId = userId.Value
        });
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

    private int? GetAuthenticatedUserId()
    {
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.TryParse(rawUserId, out var userId) ? userId : null;
    }
}
