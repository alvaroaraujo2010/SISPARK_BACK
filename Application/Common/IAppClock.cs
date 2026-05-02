namespace sispark_api.Application.Common;

public interface IAppClock
{
    DateTime UtcNow { get; }
}

public sealed class SystemAppClock : IAppClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
