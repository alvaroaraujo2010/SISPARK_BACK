using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sispark_api.Application.Parking;
using sispark_api.Contracts.Parking;
using sispark_api.Infrastructure.Auth;

namespace sispark_api.Controllers;

[ApiController]
[Route("api/parking")]
public class ParkingController(IParkingService parkingService) : ControllerBase
{
    [Authorize(Policy = AuthorizationPolicies.ParkingOperation)]
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ActiveVehicleResponse>>> GetActiveVehicles(CancellationToken cancellationToken)
        => Ok(await parkingService.GetActiveVehiclesAsync(cancellationToken));

    [Authorize(Policy = AuthorizationPolicies.ParkingOperation)]
    [HttpPost("entry-exit")]
    public async Task<ActionResult<ParkingMovementResult>> RegisterEntryOrExit(
        ParkingMovementRequest request,
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

        return Ok(await parkingService.RegisterEntryOrExitAsync(request.Plate, userId.Value, cancellationToken));
    }

    [Authorize(Policy = AuthorizationPolicies.AdministrativeAccess)]
    [HttpPost("registrations/preview")]
    public ActionResult<ParkingRegistrationPreviewResponse> PreviewRegistration(ParkingRegistrationPreviewRequest request)
        => Ok(parkingService.PreviewRegistration(request));

    [Authorize(Policy = AuthorizationPolicies.AdministrativeAccess)]
    [HttpGet("monthly-vehicles")]
    public async Task<ActionResult<IEnumerable<MonthlyVehicleResponse>>> GetMonthlyVehicles(CancellationToken cancellationToken)
        => Ok(await parkingService.GetMonthlyVehiclesAsync(cancellationToken));
}
