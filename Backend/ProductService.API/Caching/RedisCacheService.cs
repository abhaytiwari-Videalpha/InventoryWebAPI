using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace ProductService.API.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private const string CacheVersionKey = "products_cache_version";

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

    public async Task<int> GetCacheVersionAsync()
    {
        var versionString =
            await _cache.GetStringAsync(CacheVersionKey);

        if (string.IsNullOrEmpty(versionString))
        {
            await _cache.SetStringAsync(
                CacheVersionKey,
                "1");

            return 1;
        }

        return int.Parse(versionString);
    }

    public async Task IncrementCacheVersionAsync()
        {
            var currentVersion =
                await GetCacheVersionAsync();

            await _cache.SetStringAsync(
                CacheVersionKey,
                (currentVersion + 1).ToString());
        }
}