using Application.Models;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Application.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>
{
    public class Handler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly StoreContext _dbContext;
        private readonly IDistributedCache _cache;

        public Handler(StoreContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"product_{request.Id}";
            var cachedProduct = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedProduct != null)
            {
                return JsonConvert.DeserializeObject<ProductDto>(Encoding.UTF8.GetString(cachedProduct));
            }

            var product = await _dbContext.Products.FindAsync(new object?[] { request.Id },
                                                              cancellationToken: cancellationToken) 
                                                    ?? throw new Exception("Product not found");
            var productDto = new ProductDto
            (
                Id: product.Id,
                Title: product.Title,
                InventoryCount: product.InventoryCount,
                Price: product.Price - (product.Price * product.Discount / 100),
                Discount: product.Discount
            );

            var serializedProduct = JsonConvert.SerializeObject(productDto);
            await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(serializedProduct), token: cancellationToken);

            return productDto;
        }
    }
}