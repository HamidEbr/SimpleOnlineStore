using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>, ICacheable
{
    public string CacheKey => $"{nameof(Product)}_{Id}";

    public class Validator : AbstractValidator<GetProductByIdQuery>
    {
        public Validator() => RuleFor(v => v.Id).NotEmpty();
    }

    public class Handler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly StoreContext _dbContext;

        public Handler(StoreContext dbContext) => _dbContext = dbContext;

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object?[] { request.Id },
                                                              cancellationToken: cancellationToken)
                                                    ?? throw new EntityNotFoundException<Product>(request.Id);
            return new ProductDto
            (
                Id: product.Id,
                Title: product.Title,
                InventoryCount: product.InventoryCount,
                Price: product.Price,
                Discount: product.Discount
            );
        }
    }
}