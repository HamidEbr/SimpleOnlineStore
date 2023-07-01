using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Persistance;

internal class RedisStorage : ICacheStorage
{
    private readonly IDistributedCache _cache;

    public RedisStorage(IDistributedCache cache) => _cache = cache;

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> itemFactory, DateTimeOffset? expiry, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetAsync(key, cancellationToken);
        if (value is not null)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value));
        }

        var item = await itemFactory();
        if (item is not ICollection || (item is ICollection collection && collection.Count > 0))
        {
            string result = JsonConvert.SerializeObject(item);
            await StringSetAsync(key, result, expiry, cancellationToken);
        }
        return item;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) => _cache.RemoveAsync(key, cancellationToken);

    public Task StringSetAsync(string key, string value, DateTimeOffset? expiry = null, CancellationToken cancellationToken = default)
        => _cache.SetAsync(key, Encoding.UTF8.GetBytes(value), new DistributedCacheEntryOptions() { AbsoluteExpiration = expiry }, cancellationToken);

    public bool TryGetValue<T>(string cacheKey, out T cachedResult)
    {
        var value = _cache.Get(cacheKey);
        if (value is null)
        {
            cachedResult = default;
            return false;
        }
        cachedResult = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value));
        return true;
    }
}
