using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using sispark_api.Configuration;
using sispark_api.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace sispark_api.Services;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions options = jwtOptions.Value;

    public (string token, DateTime expiresAt) CreateToken(Usuario user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UsuarioLogin),
            new(ClaimTypes.Name, $"{user.Nombre} {user.Apellido}".Trim()),
            new(ClaimTypes.NameIdentifier, user.IdUsuario.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Rol?.Nombre))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Rol.Nombre));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
