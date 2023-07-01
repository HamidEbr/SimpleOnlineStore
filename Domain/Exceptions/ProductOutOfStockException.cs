namespace Domain.Exceptions;

public class ProductOutOfStockException : Exception
{
    public ProductOutOfStockException(Guid id)
        : base($"Product with ID '{id}' was out of stock now.")
    {
    }
}
