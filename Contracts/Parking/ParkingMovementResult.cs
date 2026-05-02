namespace sispark_api.Contracts.Parking;

public record ParkingMovementResult(
    bool Success,
    string Action,
    string Message,
    decimal? TotalToPay = null);
