using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sispark_api.Application.Auth;
using sispark_api.Contracts.Auth;

namespace sispark_api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth-login")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));
}
