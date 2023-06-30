namespace Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int InventoryCount { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
}
