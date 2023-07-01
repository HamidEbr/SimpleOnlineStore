namespace Domain.Interfaces;

public interface ICacheStorage
{
    public Task StringSetAsync(string key, string value, DateTimeOffset? expiry = null, CancellationToken cancellationToken = default);
    public Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> addItemFactory, DateTimeOffset? expiry, CancellationToken cancellationToken = default);
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    public bool TryGetValue<T>(string cacheKey, out T cachedResult);
}
