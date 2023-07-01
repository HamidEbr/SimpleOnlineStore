namespace Domain.Events;

public sealed record UpdateProductEvent : ProductEvent
{
    public UpdateProductEvent(Guid productId, string title, decimal price, decimal discount) : base(productId)
    {
        Title = title;
        Price = price;
        Discount = discount;
    }

    public string Title { get; }
    public decimal Price { get; }
    public decimal Discount { get; }
}
