using TaskManagementSystem.Utilities.Providers.Interfaces;

namespace TaskManagementSystem.Utilities.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}