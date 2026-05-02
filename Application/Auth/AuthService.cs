using Microsoft.EntityFrameworkCore;
using sispark_api.Application.Common;
using sispark_api.Contracts.Auth;
using sispark_api.Domain.Catalog;
using sispark_api.Domain.Entities;
using sispark_api.Infrastructure.Data;
using sispark_api.Services;

namespace sispark_api.Application.Auth;

public sealed class AuthService(
    SisparkDbContext dbContext,
    JwtTokenService tokenService,
    IAppClock clock) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();
        var user = await dbContext.Usuarios
            .Include(item => item.Rol)
            .Include(item => item.Estado)
            .FirstOrDefaultAsync(
                item => item.UsuarioLogin.ToLower() == normalizedUsername,
                cancellationToken);

        if (user is null
            || !IsActiveUser(user)
            || string.IsNullOrWhiteSpace(user.Contrasena)
            || !BCrypt.Net.BCrypt.Verify(request.Password, user.Contrasena))
        {
            throw new CredentialsRejectedException("Credenciales invalidas o usuario inactivo.");
        }

        user.UltimoAcceso = clock.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = tokenService.CreateToken(user);

        return new LoginResponse(
            token,
            expiresAt,
            $"{user.Nombre} {user.Apellido}".Trim(),
            user.UsuarioLogin,
            user.Rol?.Nombre ?? string.Empty);
    }

    private static bool IsActiveUser(Usuario user)
        => user.Estado?.Modulo == EstadoModulos.General && user.Estado.Nombre == EstadoNombres.Activo;
}
