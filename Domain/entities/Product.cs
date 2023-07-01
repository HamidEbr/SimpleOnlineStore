using Domain.Events;

namespace Domain.Entities;

public class Product
{
    private Product()
    {
    }

    public Product(string title, decimal price, decimal discount)
    {
        Title = title;
        Price = price;
        Discount = discount;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; }
    // The current inventory count is computed from the event history
    public int InventoryCount { get; private set; }
    public decimal Price { get; private set; }
    public decimal Discount { get; private set; }

    // The event history is a list of events that have been applied to the entity
    private readonly List<ProductEvent> _eventHistory = new();

    // Applies an event to the entity
    public void ApplyEvent(ProductEvent ev)
    {
        switch (ev)
        {
            case ProductPurchasedEvent:
                InventoryCount--;
                break;
            case IncreaseProductInventoryEvent increaseProductInventoryEvent:
                InventoryCount += increaseProductInventoryEvent.Quantity;
                break;
            case UpdateProductEvent updateProductEvent:
                Title = updateProductEvent.Title;
                Price = updateProductEvent.Price;
                Discount = updateProductEvent.Discount;
                break;
        }

        _eventHistory.Add(ev);
    }
}
