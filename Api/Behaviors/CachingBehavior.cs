using Domain.Interfaces;
using MediatR;

namespace Api.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : ICacheable, IRequest<TResponse>
{
    private readonly ICacheStorage _cache;

    public CachingBehavior(ICacheStorage cache)
        => _cache = cache;

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => _cache.GetOrAddAsync(key: request.CacheKey, addItemFactory: () => next(), expiry: request.AbsoluteExpiration);
}
