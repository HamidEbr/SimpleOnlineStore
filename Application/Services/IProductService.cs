using Application.Models;
using System.Threading.Tasks;

namespace Application.Services;

public interface IProductService
{
    Task<Guid> AddProductAsync(
        string title,
        int inventoryCount,
        decimal price,
        decimal discount,
        CancellationToken cancellationToken);

    Task<ProductDto> GetProductAsync(Guid guid, CancellationToken cancellationToken);
    Task UpdateProductAsync(Guid productId, UpdateProductDto productDto, CancellationToken cancellationToken);
}
