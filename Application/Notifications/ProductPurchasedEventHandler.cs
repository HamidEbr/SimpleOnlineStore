﻿using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Notifications;

internal class ProductPurchasedEventHandler : INotificationHandler<ProductPurchasedEvent>
{
    private readonly StoreContext _dbContext;
    private readonly IDistributedCache _cache;

    public ProductPurchasedEventHandler(StoreContext dbContext, IDistributedCache cache)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task Handle(ProductPurchasedEvent ev, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { ev.ProductId }, cancellationToken: cancellationToken) ?? throw new EntityNotFoundException<Product>(ev.ProductId);

        // Decrement the inventory count of the product
        product.ApplyEvent(ev);
        await _dbContext.SaveChangesAsync(cancellationToken);
        // Remove the cached product since its inventory count has changed
        var cacheKey = $"{nameof(Product)}_{product.Id}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
    }
}
