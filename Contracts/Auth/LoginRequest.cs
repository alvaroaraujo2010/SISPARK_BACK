using System.ComponentModel.DataAnnotations;

namespace sispark_api.Contracts.Auth;

public record LoginRequest(
    [Required][MaxLength(50)] string Username,
    [Required][MaxLength(100)] string Password);
