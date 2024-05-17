using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.Dal.Settings;

namespace TaskManagementSystem.Dal.Repositories;

public class RateLimiterRepository : RedisRepository, IRateLimiterRepository
{
    public RateLimiterRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value) { }

    protected override string KeyPrefix { get; } = "rate_limit";
    
    public async Task<long> GetRequestsNumber(string clientIp, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var connection = await GetConnection();

        var key = GetKey(clientIp);
        if (!connection.KeyExists(key))
        {
            connection.StringSet(key, 0, TimeSpan.FromMinutes(1), When.NotExists);
        }
        
        return connection.StringIncrement(key);
    }
}