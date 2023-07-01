using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Notifications;

internal class ProductPurchasedEventHandler : INotificationHandler<ProductPurchasedEvent>
{
    private readonly StoreContext _dbContext;

    public ProductPurchasedEventHandler(StoreContext dbContext) => _dbContext = dbContext;

    public async Task Handle(ProductPurchasedEvent ev, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { ev.ProductId }, cancellationToken: cancellationToken) ?? throw new EntityNotFoundException<Product>(ev.ProductId);

        // Decrement the inventory count of the product
        product.ApplyEvent(ev);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
