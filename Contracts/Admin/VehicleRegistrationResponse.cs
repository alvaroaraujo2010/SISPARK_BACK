namespace sispark_api.Contracts.Admin;

public record VehicleRegistrationResponse(
    bool Success,
    string Message,
    int ClientId,
    int VehicleId,
    int UserId);
