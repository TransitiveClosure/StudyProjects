namespace TaskManagementSystem.Bll.Services.Interfaces;

public interface IRateLimiterService
{
    Task ThrowIfTooManyRequests(string clientIp, CancellationToken cancellationToken);
}