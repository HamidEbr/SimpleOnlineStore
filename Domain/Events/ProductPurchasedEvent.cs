using Domain.Entities;

namespace Domain.Events;

// Event for when a product is purchased
public sealed record ProductPurchasedEvent : ProductEvent
{
    public User Buyer { get; }
    public int Quantity { get; }

    public ProductPurchasedEvent(Guid productId, User buyer, int quantity) : base(productId)
    {
        Buyer = buyer;
        Quantity = quantity;
    }
}
