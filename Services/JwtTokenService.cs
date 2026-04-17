using Microsoft.IdentityModel.Tokens;
using sispark_api.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace sispark_api.Services;

public class JwtTokenService(IConfiguration configuration)
{
    public (string token, DateTime expiresAt) CreateToken(Usuario user)
    {
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;
        var key = configuration["Jwt:Key"]!;
        var expirationMinutes = configuration.GetValue<int>("Jwt:ExpirationMinutes");
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UsuarioLogin),
            new(ClaimTypes.Name, $"{user.Nombre} {user.Apellido}".Trim()),
            new(ClaimTypes.NameIdentifier, user.IdUsuario.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
