namespace sispark_api.Contracts.Parking;

public record ActiveVehicleResponse(
    int IdRegistro,
    string Placa,
    DateTime FechaIngreso,
    DateTime? FechaSalida,
    decimal ValorPagar,
    string TipoServicio);
