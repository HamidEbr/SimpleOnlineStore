using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services;

internal class OrderService : IOrderService
{
    private readonly StoreContext _dbContext;
    private readonly IMediator _mediator;

    public OrderService(StoreContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task AddOrderAsync(Guid productId, Guid userId, int quantity, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { productId },
                                                  cancellationToken: cancellationToken)
                                        ?? throw new EntityNotFoundException<Product>(productId);
        if (product.InventoryCount <= 0)
        {
            throw new ProductOutOfStockException(product.Id);
        }

        var user = await _dbContext.Users.FindAsync(new object?[] { userId },
                                                    cancellationToken: cancellationToken)
                                                ?? throw new EntityNotFoundException<User>(userId);
        var order = new Order
        {
            Product = product,
            CreationDate = DateTime.UtcNow,
            Buyer = user,
            Quantity = quantity
        };

        user.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var productPurchasedEvent = new ProductPurchasedEvent(productId, user, quantity);
        await _mediator.Publish(productPurchasedEvent, cancellationToken);
    }
}
