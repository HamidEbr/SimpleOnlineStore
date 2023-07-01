namespace Domain.Events;

// Event for when a product is restocked
public sealed record IncreaseProductInventoryEvent : ProductEvent
{
    public int Quantity { get; }

    public IncreaseProductInventoryEvent(Guid productId, int quantity) : base(productId) => Quantity = quantity;
}