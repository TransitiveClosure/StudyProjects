using StackExchange.Redis;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.Dal.Settings;

namespace TaskManagementSystem.Dal.Repositories;

public abstract class RedisRepository : IRedisRepository
{
    private static ConnectionMultiplexer? _connection;
    
    private readonly DalOptions _dalSettings;

    protected RedisRepository(DalOptions dalSettings)
    {
        _dalSettings = dalSettings;
    }

    protected abstract string KeyPrefix { get; }
    
    protected virtual TimeSpan KeyTtl => TimeSpan.MaxValue;
    
    protected async Task<IDatabase> GetConnection()
    {
        _connection ??= await ConnectionMultiplexer.ConnectAsync(_dalSettings.RedisConnectionString);
        
        return _connection.GetDatabase();
    }
    
    protected RedisKey GetKey(params object[] identifiers)
        => new ($"{KeyPrefix}:{string.Join(':', identifiers)}");
}