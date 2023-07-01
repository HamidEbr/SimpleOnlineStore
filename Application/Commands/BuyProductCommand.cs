using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Commands;

public sealed record BuyProductCommand(Guid ProductId, Guid UserId, int Quantity) : IRequest
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
        private readonly IMediator _mediator;

        public Handler(StoreContext dbContext, IDistributedCache cache, IMediator mediator)
        {
            _dbContext = dbContext;
            _cache = cache;
            _mediator = mediator;
        }

        public async Task Handle(BuyProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object?[] { request.ProductId },
                                                              cancellationToken: cancellationToken)
                                                    ?? throw new EntityNotFoundException<Product>(request.ProductId);
            if (product.InventoryCount <= 0)
            {
                throw new ProductOutOfStockException(product.Id);
            }

            var user = await _dbContext.Users.FindAsync(new object?[] { request.UserId },
                                                        cancellationToken: cancellationToken)
                                                    ?? throw new EntityNotFoundException<User>(request.UserId);
            var order = new Order
            {
                Product = product,
                CreationDate = DateTime.UtcNow,
                Buyer = user,
                Quantity = request.Quantity
            };

            user.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var productPurchasedEvent = new ProductPurchasedEvent(request.ProductId, user, request.Quantity);
            await _mediator.Publish(productPurchasedEvent, cancellationToken);

            // Remove the cached product since its inventory count has changed
            var cacheKey = $"product_{product.Id}";
            await _cache.RemoveAsync(cacheKey, cancellationToken);
        }
    }
}