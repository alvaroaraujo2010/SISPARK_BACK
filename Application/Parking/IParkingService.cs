using sispark_api.Contracts.Parking;

namespace sispark_api.Application.Parking;

public interface IParkingService
{
    Task<IReadOnlyList<ActiveVehicleResponse>> GetActiveVehiclesAsync(CancellationToken cancellationToken);

    Task<ParkingMovementResult> RegisterEntryOrExitAsync(string plate, int userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MonthlyVehicleResponse>> GetMonthlyVehiclesAsync(CancellationToken cancellationToken);

    ParkingRegistrationPreviewResponse PreviewRegistration(ParkingRegistrationPreviewRequest request);
}
