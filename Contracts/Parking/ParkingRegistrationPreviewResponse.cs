namespace sispark_api.Contracts.Parking;

public record ParkingRegistrationPreviewResponse(
    bool Success,
    string Message,
    string FullName,
    string Plate,
    string PaymentType);
