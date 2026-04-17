namespace sispark_api.Contracts.Admin;

public record DashboardSummaryResponse(
    int ActiveVehicles,
    int AvailableSpots,
    decimal DailyRevenue,
    int MonthlyDueSoon);
