using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Commands;

public sealed record BuyProductCommand(Guid ProductId, Guid UserId) : IRequest
{
    public class Validator : AbstractValidator<BuyProductCommand>
    {
        public Validator()
        {
            RuleFor(v => v.ProductId).NotEmpty();
            RuleFor(v => v.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<BuyProductCommand>
    {
        private readonly StoreContext _dbContext;
        private readonly IDistributedCache _cache;

        public Handler(StoreContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task Handle(BuyProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object?[] { request.ProductId },
                                                              cancellationToken: cancellationToken)
                                                    ?? throw new Exception("Product not found");
            if (product.InventoryCount <= 0)
            {
                throw new Exception("Product is out of stock");
            }

            product.InventoryCount--;

            var user = await _dbContext.Users.FindAsync(new object?[] { request.UserId },
                                                        cancellationToken: cancellationToken)
                                                    ?? throw new Exception("User not found");
            var order = new Order
            {
                Product = product,
                CreationDate = DateTime.UtcNow,
                Buyer = user
            };

            user.Orders.Add(order);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Remove the cached product since its inventory count has changed
            var cacheKey = $"product_{product.Id}";
            await _cache.RemoveAsync(cacheKey, cancellationToken);
        }
    }
}