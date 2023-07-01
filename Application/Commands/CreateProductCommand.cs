using Application.Services;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public sealed record CreateProductCommand(
    string Title,
    int InventoryCount,
    decimal Price,
    decimal Discount) : IRequest<Guid>
{
    public class Validator : AbstractValidator<CreateProductCommand>
    {
        private readonly StoreContext _context;

        public Validator(StoreContext context)
        {
            _context = context;
            RuleFor(v => v.Title).NotEmpty();
            RuleFor(v => v.Title).MaximumLength(40);
            RuleFor(v => v.Price).GreaterThan(0);
            RuleFor(p => p)
                    .MustAsync(IsProductTitleDuplicatedAsync)
                    .WithMessage("Product title is duplicated");
        }

        private Task<bool> IsProductTitleDuplicatedAsync(
            CreateProductCommand arg1,
            CancellationToken cancellationToken)
            => _context.Products.AnyAsync(p => p.Title == arg1.Title);
    }

    public class Handler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductService _productService;

        public Handler(IProductService productService) => _productService = productService;

        public Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            => _productService.AddProductAsync(command.Title, command.InventoryCount, command.Price, command.Discount, cancellationToken);
    }

}
