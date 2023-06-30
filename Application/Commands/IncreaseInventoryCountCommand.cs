using Infrastructure.Persistance;
using MediatR;

namespace Application.Commands;

public sealed record IncreaseInventoryCountCommand(int ProductId, int Count) : IRequest
{
    public class Handler : IRequestHandler<IncreaseInventoryCountCommand>
    {
        private readonly StoreContext _dbContext;

        public Handler(StoreContext dbContext) => _dbContext = dbContext;

        public async Task Handle(IncreaseInventoryCountCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object?[] { request.ProductId }, cancellationToken: cancellationToken);

            if (product is null)
            {
                throw new Exception("Product not found");
            }

            product.InventoryCount += request.Count;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
