using Application.Models;
using Application.Services;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public sealed record class UpdateProductCommand(Guid ProductId, UpdateProductDto ProductDto) : IRequest
{
    public class Validator : AbstractValidator<UpdateProductCommand>
    {
        private readonly StoreContext _context;

        public Validator(StoreContext context)
        {
            _context = context;
            RuleFor(v => v.ProductDto.Title).NotEmpty();
            RuleFor(v => v.ProductDto.Title).MaximumLength(40);
            RuleFor(v => v.ProductDto.Price).GreaterThan(0);
            RuleFor(p => p)
                        .MustAsync(IsProductTitleDuplicatedAsync)
                        .WithMessage("Product title is duplicated");
        }

        private Task<bool> IsProductTitleDuplicatedAsync(
            UpdateProductCommand arg1,
            CancellationToken cancellationToken)
            => _context.Products.AnyAsync(p => p.Title == arg1.ProductDto.Title, cancellationToken: cancellationToken);
    }

    public class Handler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductService _productService;

        public Handler(IProductService productService) => _productService = productService;

        public Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
            => _productService.UpdateProductAsync(request.ProductId, request.ProductDto, cancellationToken);
    }
}
