namespace TaskManagementSystem.Utilities.Providers.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow();
}