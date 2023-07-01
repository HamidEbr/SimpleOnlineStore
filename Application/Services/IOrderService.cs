namespace Application.Services;

public interface IOrderService
{
    Task AddOrderAsync(Guid productId, Guid userId, int quantity, CancellationToken cancellationToken);
}
