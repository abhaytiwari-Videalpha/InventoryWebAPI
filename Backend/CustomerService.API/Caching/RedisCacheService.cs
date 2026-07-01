using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace CustomerService.API.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetData<T>(string key)
    {
        var value = await _cache.GetStringAsync(key);

        if (value == null)
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetData<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiration ?? TimeSpan.FromMinutes(5)
            };

        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value),
            options);
    }

    public async Task RemoveData(string key)
    {
        await _cache.RemoveAsync(key);
    }
}