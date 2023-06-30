using Domain.Entities;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Commands;

public sealed record CreateProductCommand(
    string Title,
    int InventoryCount,
    decimal Price,
    decimal Discount) : IRequest<Guid>
{
    public class Handler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly StoreContext _dbContext;

        public Handler(StoreContext dbContext) => _dbContext = dbContext;

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Title.Length > 40)
            {
                throw new Exception("Product title must be less than 40 characters");
            }

            if (_dbContext.Products.Any(p => p.Title == request.Title))
            {
                throw new Exception("Product title must be unique");
            }

            var product = new Product
            {
                Title = request.Title,
                InventoryCount = request.InventoryCount,
                Price = request.Price,
                Discount = request.Discount
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }

}
