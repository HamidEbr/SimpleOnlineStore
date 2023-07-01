namespace Domain.Events;

// Event for when a product is restocked
public sealed record ProductRestockedEvent : ProductEvent
{
    public int Quantity { get; }

    public ProductRestockedEvent(Guid productId, int quantity) : base(productId) => Quantity = quantity;
}