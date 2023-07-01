using Application.Models;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
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
        private readonly IProductService _productService;

        public Handler(IProductService productService) => _productService = productService;

        public Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
            => _productService.GetProductAsync(request.Id, cancellationToken);
    }
}