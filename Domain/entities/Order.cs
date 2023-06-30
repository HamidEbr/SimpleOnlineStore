namespace Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public Guid BuyerId { get; set; }
    public User Buyer { get; set; }
    public int Quantity { get; set; }
}
