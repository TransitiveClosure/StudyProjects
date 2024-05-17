using TaskManagementSystem.Bll.Exceptions;
using TaskManagementSystem.Bll.Services.Interfaces;
using TaskManagementSystem.Dal.Repositories.Interfaces;

namespace TaskManagementSystem.Bll.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly IRateLimiterRepository _rateLimiterRepository;

    private const int RequestsPerMinute = 100;
    
    public RateLimiterService(IRateLimiterRepository rateLimiterRepository)
    {
        _rateLimiterRepository = rateLimiterRepository;
    }
    
    public async Task ThrowIfTooManyRequests(string clientIp, CancellationToken cancellationToken)
    {
        var actualScore = await _rateLimiterRepository.GetRequestsNumber(clientIp, cancellationToken);
        
        if (actualScore < RequestsPerMinute) return;

        throw new TooManyRequestsException(clientIp);
    }
}