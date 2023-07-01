using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Notifications;

internal class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly StoreContext _dbContext;

    public ProductCreatedEventHandler(StoreContext dbContext) => _dbContext = dbContext;

    public async Task Handle(ProductCreatedEvent ev, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { ev.ProductId }, cancellationToken: cancellationToken) ?? throw new EntityNotFoundException<Product>(ev.ProductId);

        // Decrement the inventory count of the product
        product.ApplyEvent(ev);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
