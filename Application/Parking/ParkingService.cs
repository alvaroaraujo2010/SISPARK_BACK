using System.Data;
using Microsoft.EntityFrameworkCore;
using sispark_api.Application.Common;
using sispark_api.Contracts.Parking;
using sispark_api.Domain.Catalog;
using sispark_api.Domain.Entities;
using sispark_api.Infrastructure.Data;

namespace sispark_api.Application.Parking;

public sealed class ParkingService(SisparkDbContext dbContext, IAppClock clock) : IParkingService
{
    public async Task<IReadOnlyList<ActiveVehicleResponse>> GetActiveVehiclesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.RegistrosParqueo
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
    }

    public Task<ParkingMovementResult> RegisterEntryOrExitAsync(
        string plate,
        int userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            throw new ArgumentException("La placa es obligatoria.", nameof(plate));
        }

        var normalizedPlate = plate.Trim().ToUpperInvariant();
        var strategy = dbContext.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            var vehicle = await dbContext.Vehiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Placa == normalizedPlate, cancellationToken);

            if (vehicle is null)
            {
                throw new ResourceNotFoundException("La placa no esta registrada en la base de vehiculos.");
            }

            var activeRecord = await dbContext.RegistrosParqueo
                .FirstOrDefaultAsync(
                    item => item.IdVehiculo == vehicle.IdVehiculo && item.FechaSalida == null,
                    cancellationToken);

            if (activeRecord is null)
            {
                var service = await dbContext.TiposServicio
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        item => item.Nombre == TipoServicioNombres.PorHora,
                        cancellationToken);

                if (service is null)
                {
                    throw new BusinessRuleException("No existe el tipo de servicio por hora.");
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
                    throw new BusinessRuleException("No existe una tarifa activa para este vehiculo.");
                }

                var openedStateId = await EstadoQueries.GetStateIdAsync(
                    dbContext,
                    EstadoModulos.Registro,
                    EstadoNombres.Abierto,
                    cancellationToken);

                var newRecord = new RegistroParqueo
                {
                    IdVehiculo = vehicle.IdVehiculo,
                    IdTipoServicio = service.IdTipoServicio,
                    IdTarifa = tarifa.IdTarifa,
                    FechaIngreso = clock.UtcNow,
                    IdUsuarioIngreso = userId,
                    ValorCalculado = 0,
                    ValorFinal = 0,
                    IdEstado = openedStateId,
                };

                dbContext.RegistrosParqueo.Add(newRecord);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new ParkingMovementResult(true, "entry", "Vehiculo registrado correctamente.");
            }

            var exitDate = clock.UtcNow;
            var elapsedMinutes = Math.Max(1, (int)Math.Ceiling((exitDate - activeRecord.FechaIngreso).TotalMinutes));
            var totalToPay = await CalculateParkingTotalAsync(activeRecord.IdTarifa, elapsedMinutes, cancellationToken);
            var closedStateId = await EstadoQueries.GetStateIdAsync(
                dbContext,
                EstadoModulos.Registro,
                EstadoNombres.Cerrado,
                cancellationToken);

            activeRecord.FechaSalida = exitDate;
            activeRecord.MinutosConsumidos = elapsedMinutes;
            activeRecord.ValorCalculado = totalToPay;
            activeRecord.ValorFinal = totalToPay;
            activeRecord.IdUsuarioSalida = userId;
            activeRecord.IdEstado = closedStateId;

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ParkingMovementResult(
                true,
                "exit",
                "Salida registrada correctamente.",
                totalToPay);
        });
    }

    public async Task<IReadOnlyList<MonthlyVehicleResponse>> GetMonthlyVehiclesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Mensualidades
            .AsNoTracking()
            .Include(item => item.Vehiculo)
            .Include(item => item.Cliente)
            .OrderBy(item => item.FechaFin)
            .Select(item => new MonthlyVehicleResponse(
                item.IdMensualidad,
                item.Cliente != null ? item.Cliente.NombreCompleto : string.Empty,
                item.Vehiculo != null ? item.Vehiculo.Placa : string.Empty,
                item.FechaInicio,
                item.FechaFin,
                item.Valor,
                item.IdEstado))
            .ToListAsync(cancellationToken);
    }

    public ParkingRegistrationPreviewResponse PreviewRegistration(ParkingRegistrationPreviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Plate) || string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("Nombre completo y placa son obligatorios.");
        }

        return new ParkingRegistrationPreviewResponse(
            true,
            "Vista previa recibida correctamente.",
            request.FullName.Trim(),
            request.Plate.Trim().ToUpperInvariant(),
            request.PaymentType.Trim());
    }

    private async Task<decimal> CalculateParkingTotalAsync(int tarifaId, int elapsedMinutes, CancellationToken cancellationToken)
    {
        var tarifa = await dbContext.Tarifas
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.IdTarifa == tarifaId, cancellationToken);

        if (tarifa is null)
        {
            throw new BusinessRuleException("No se encontro la tarifa asociada al registro.");
        }

        var fractionMinutes = tarifa.FraccionMinutos.GetValueOrDefault(60);
        var units = Math.Max(1, (int)Math.Ceiling(elapsedMinutes / (decimal)fractionMinutes));
        return units * tarifa.Valor;
    }
}
