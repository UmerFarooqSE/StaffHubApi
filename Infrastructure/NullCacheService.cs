namespace StaffHubApi.Infrastructure;

public class NullCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key)
        => Task.FromResult<T?>(default);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        => Task.CompletedTask;

    public Task RemoveAsync(string key)
        => Task.CompletedTask;

    public Task RemoveByPrefixAsync(string prefix)
        => Task.CompletedTask;
}