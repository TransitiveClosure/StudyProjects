namespace TaskManagementSystem.Dal.Repositories.Interfaces;

public interface IUserRateLimitRepository
{
    Task<long> IncRequestPerMinute(long userId, CancellationToken token);
}