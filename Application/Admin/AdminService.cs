using Microsoft.EntityFrameworkCore;
using sispark_api.Application.Common;
using sispark_api.Contracts.Admin;
using sispark_api.Domain.Catalog;
using sispark_api.Domain.Entities;
using sispark_api.Infrastructure.Data;

namespace sispark_api.Application.Admin;

public sealed class AdminService(SisparkDbContext dbContext, IAppClock clock) : IAdminService
{
    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(CancellationToken cancellationToken)
    {
        var today = clock.UtcNow.Date;
        var weekLimit = DateOnly.FromDateTime(today.AddDays(7));

        var activeVehicles = await dbContext.RegistrosParqueo
            .AsNoTracking()
            .CountAsync(item => item.FechaSalida == null, cancellationToken);

        var availableStateId = await EstadoQueries.GetStateIdAsync(
            dbContext,
            EstadoModulos.Celda,
            EstadoNombres.Disponible,
            cancellationToken);

        var availableSpots = await dbContext.Celdas
            .AsNoTracking()
            .CountAsync(item => item.IdEstado == availableStateId, cancellationToken);

        var dailyRevenue = await dbContext.RegistrosParqueo
            .AsNoTracking()
            .Where(item => item.FechaSalida != null && item.FechaSalida >= today)
            .SumAsync(item => (decimal?)item.ValorFinal, cancellationToken) ?? 0;

        var vigenteStateId = await EstadoQueries.GetStateIdAsync(
            dbContext,
            EstadoModulos.Mensualidad,
            EstadoNombres.Vigente,
            cancellationToken);

        var monthlyDueSoon = await dbContext.Mensualidades
            .AsNoTracking()
            .CountAsync(
                item => item.IdEstado == vigenteStateId
                    && item.FechaFin >= DateOnly.FromDateTime(today)
                    && item.FechaFin <= weekLimit,
                cancellationToken);

        return new DashboardSummaryResponse(activeVehicles, availableSpots, dailyRevenue, monthlyDueSoon);
    }

    public async Task<IReadOnlyList<VehicleTypeResponse>> GetVehicleTypesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.TiposVehiculo
            .AsNoTracking()
            .OrderBy(item => item.Nombre)
            .Select(item => new VehicleTypeResponse(item.IdTipoVehiculo, item.Nombre))
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleRegistrationResponse> RegisterVehicleAsync(
        VehicleRegistrationRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)
            || string.IsNullOrWhiteSpace(request.Plate)
            || request.VehicleTypeId <= 0)
        {
            throw new ArgumentException("Los datos del cliente, placa y tipo de vehiculo son obligatorios.");
        }

        var plate = request.Plate.Trim().ToUpperInvariant();
        var existingVehicle = await dbContext.Vehiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Placa == plate, cancellationToken);

        if (existingVehicle is not null)
        {
            throw new ConflictResourceException("La placa ya existe en el sistema.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var activeStateId = await EstadoQueries.GetStateIdAsync(
            dbContext,
            EstadoModulos.General,
            EstadoNombres.Activo,
            cancellationToken);

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
                FechaCreacion = clock.UtcNow,
                IdUsuarioCreacion = userId,
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
            existingClient.FechaActualizacion = clock.UtcNow;
            existingClient.IdUsuarioActualizacion = userId;
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
            FechaCreacion = clock.UtcNow,
            IdUsuarioCreacion = userId,
        };

        dbContext.Vehiculos.Add(vehicle);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new VehicleRegistrationResponse(
            true,
            "Cliente y vehiculo registrados correctamente.",
            existingClient.IdCliente,
            vehicle.IdVehiculo,
            userId);
    }
}
