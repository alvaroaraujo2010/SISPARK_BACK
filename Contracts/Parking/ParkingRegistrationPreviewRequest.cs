using System.ComponentModel.DataAnnotations;

namespace sispark_api.Contracts.Parking;

public class ParkingRegistrationPreviewRequest
{
    [Required]
    [MaxLength(20)]
    public string IdentificationType { get; set; } = string.Empty;
    [Required]
    [MaxLength(30)]
    public string IdentificationNumber { get; set; } = string.Empty;
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MaxLength(120)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;
    [Required]
    [MaxLength(15)]
    public string Plate { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string VehicleModel { get; set; } = string.Empty;
    [Required]
    [MaxLength(20)]
    public string PaymentType { get; set; } = string.Empty;
    [MaxLength(255)]
    public string? Comments { get; set; }
}
