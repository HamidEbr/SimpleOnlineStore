using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Notifications;

public class ProductRestockedEventHandler : INotificationHandler<ProductRestockedEvent>
{
    private readonly StoreContext _dbContext;

    public ProductRestockedEventHandler(StoreContext dbContext) => _dbContext = dbContext;

    public async Task Handle(ProductRestockedEvent ev, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { ev.ProductId }, cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException<Product>(ev.ProductId);

        // Apply the ProductRestockedEvent to the Product entity
        product.ApplyEvent(ev);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
