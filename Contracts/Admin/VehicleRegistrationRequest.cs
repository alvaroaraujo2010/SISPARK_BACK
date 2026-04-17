namespace sispark_api.Contracts.Admin;

public class VehicleRegistrationRequest
{
    public string IdentificationType { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int VehicleTypeId { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string? Comments { get; set; }
}
