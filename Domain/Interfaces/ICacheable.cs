namespace Domain.Interfaces;

public interface ICacheable
{
    string CacheKey { get; }
    DateTimeOffset? AbsoluteExpiration => DateTimeOffset.Now.AddMinutes(5);
}
