namespace Domain.Events;

public sealed record ProductCreatedEvent : ProductEvent
{
    public string Title { get; init; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public int InventoryCount { get; init; }

    public ProductCreatedEvent(Guid productId) : base(productId) { }
}
