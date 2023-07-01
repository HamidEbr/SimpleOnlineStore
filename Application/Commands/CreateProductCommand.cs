using Domain.Entities;
using Domain.Events;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Commands;

public sealed record CreateProductCommand(
    string Title,
    int InventoryCount,
    decimal Price,
    decimal Discount) : IRequest<Guid>
{
    public class Validator : AbstractValidator<CreateProductCommand>
    {
        public Validator()
        {
            RuleFor(v => v.Title).NotEmpty();
            RuleFor(v => v.Title).MaximumLength(40);
            RuleFor(v => v.Price).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly StoreContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(StoreContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Title = command.Title,
                Price = command.Price,
                Discount = command.Discount,
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Publish the ProductCreatedEvent using MediatR
            var productCreatedEvent = new ProductCreatedEvent(product.Id)
            {
                Title = command.Title,
                Price = command.Price,
                Discount = command.Discount,
                InventoryCount = command.InventoryCount
            };
            await _mediator.Publish(productCreatedEvent, cancellationToken);

            return product.Id;
        }
    }

}
