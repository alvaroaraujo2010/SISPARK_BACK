using System.ComponentModel.DataAnnotations;

namespace sispark_api.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(5, 1440)]
    public int ExpirationMinutes { get; set; } = 120;
}
