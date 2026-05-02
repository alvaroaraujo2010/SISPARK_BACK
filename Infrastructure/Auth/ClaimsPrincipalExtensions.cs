using System.Security.Claims;

namespace sispark_api.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var rawUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return int.TryParse(rawUserId, out var userId) ? userId : null;
    }

    public static string? GetRoleName(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role);
}
