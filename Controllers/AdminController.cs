using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sispark_api.Application.Admin;
using sispark_api.Contracts.Admin;
using sispark_api.Infrastructure.Auth;

namespace sispark_api.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdministrativeAccess)]
[Route("api/admin")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("dashboard-summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetDashboardSummary(CancellationToken cancellationToken)
        => Ok(await adminService.GetDashboardSummaryAsync(cancellationToken));

    [HttpGet("vehicle-types")]
    public async Task<ActionResult<IEnumerable<VehicleTypeResponse>>> GetVehicleTypes(CancellationToken cancellationToken)
        => Ok(await adminService.GetVehicleTypesAsync(cancellationToken));

    [HttpPost("vehicle-registrations")]
    public async Task<ActionResult<VehicleRegistrationResponse>> RegisterVehicle(
        VehicleRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Problem(
                title: "Acceso no autorizado",
                detail: "No fue posible identificar el usuario autenticado.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(await adminService.RegisterVehicleAsync(request, userId.Value, cancellationToken));
    }
}
