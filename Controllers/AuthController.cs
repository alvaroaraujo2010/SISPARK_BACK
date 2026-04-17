using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sispark_api.Contracts.Auth;
using sispark_api.Infrastructure.Data;
using sispark_api.Services;

namespace sispark_api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(SisparkDbContext dbContext, JwtTokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username y password son obligatorios." });
        }

        var normalizedUsername = request.Username.Trim().ToLowerInvariant();
        var user = await dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.UsuarioLogin.ToLower() == normalizedUsername
                    && item.IdEstado == 1,
                cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Contrasena) || !BCrypt.Net.BCrypt.Verify(request.Password, user.Contrasena))
        {
            return Unauthorized(new { message = "Credenciales inválidas o usuario inactivo." });
        }

        var (token, expiresAt) = tokenService.CreateToken(user);

        return Ok(new LoginResponse(
            token,
            expiresAt,
            $"{user.Nombre} {user.Apellido}".Trim(),
            user.UsuarioLogin));
    }
}
