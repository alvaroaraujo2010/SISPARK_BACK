namespace sispark_api.Contracts.Parking;

public class ParkingRegistrationPreviewRequest
{
    public string IdentificationType { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string? Comments { get; set; }
}
