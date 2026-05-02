namespace sispark_api.Contracts.Parking;

public record MonthlyVehicleResponse(
    int IdMensualidad,
    string Cliente,
    string Placa,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    decimal Valor,
    int IdEstado);
