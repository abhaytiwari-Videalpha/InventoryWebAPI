using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace InvoiceService.API.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetData<T>(
        string key)
    {
        var data =
            await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(data))
        {
            return default;
        }

        return JsonSerializer
            .Deserialize<T>(data);
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
                    expiration ??
                    TimeSpan.FromMinutes(10)
            };

        var json =
            JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            options);
    }

    public async Task RemoveData(
        string key)
    {
        await _cache.RemoveAsync(key);
    }
}