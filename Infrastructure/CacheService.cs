namespace StaffHubApi.Infrastructure;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis,
        ILogger<CacheService> logger)
    {
        _cache = cache;
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cached = await _cache.GetStringAsync(key);
            if (cached is null)
            {
                _logger.LogDebug("Cache MISS for key: {Key}", key);
                return default;
            }

            _logger.LogDebug("Cache HIT for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cached);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache GET failed for key: {Key}. Falling through to database.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
            };

            var serialized = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serialized, options);
            _logger.LogDebug("Cache SET for key: {Key}, expiry: {Expiry}", key, options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache SET failed for key: {Key}. Continuing without cache.", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogDebug("Cache REMOVE for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache REMOVE failed for key: {Key}.", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {

        if (_redis is null)
        {
            await RemoveAsync(prefix);
            return;
        }

        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var instanceName = "StaffHubApi:";
            var keys = server.Keys(pattern: $"{instanceName}{prefix}*").ToArray();

            foreach (var key in keys)
            {
                await _redis.GetDatabase().KeyDeleteAsync(key);
                _logger.LogDebug("Cache REMOVE by prefix, deleted key: {Key}", key);
            }

            _logger.LogDebug("Cache REMOVE by prefix: {Prefix}, removed {Count} keys", prefix, keys.Length);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache REMOVE by prefix failed for: {Prefix}.", prefix);
        }
    }
}