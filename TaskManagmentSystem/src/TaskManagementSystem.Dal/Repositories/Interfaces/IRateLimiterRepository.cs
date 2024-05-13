namespace TaskManagementSystem.Dal.Repositories.Interfaces;

public interface IRateLimiterRepository
{
    Task<long> GetRequestsNumber(string clientIp, CancellationToken token);
}