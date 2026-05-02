using sispark_api.Contracts.Admin;

namespace sispark_api.Application.Admin;

public interface IAdminService
{
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<VehicleTypeResponse>> GetVehicleTypesAsync(CancellationToken cancellationToken);

    Task<VehicleRegistrationResponse> RegisterVehicleAsync(VehicleRegistrationRequest request, int userId, CancellationToken cancellationToken);
}
