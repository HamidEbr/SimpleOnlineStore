using Domain.Events;

namespace Domain.Entities;

public class Product
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; }
    // The current inventory count is computed from the event history
    public int InventoryCount { get; private set; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }

    // The event history is a list of events that have been applied to the entity
    private readonly List<ProductEvent> _eventHistory = new();

    // Applies an event to the entity
    public void ApplyEvent(ProductEvent ev)
    {
        switch (ev)
        {
            case ProductCreatedEvent createdEvent:
                InventoryCount = createdEvent.InventoryCount;
                break;
            case ProductPurchasedEvent:
                InventoryCount--;
                break;
            case ProductRestockedEvent restockedEvent:
                InventoryCount += restockedEvent.Quantity;
                break;
        }

        _eventHistory.Add(ev);
    }
}
