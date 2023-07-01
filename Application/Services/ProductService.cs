using Application.Models;
using Azure.Core;
using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Infrastructure.Persistance;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Services;

internal class ProductService : IProductService
{
    private readonly StoreContext _dbContext;
    private readonly IMediator _mediator;

    public ProductService(StoreContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task<Guid> AddProductAsync(
        string title,
        int inventoryCount,
        decimal price,
        decimal discount,
        CancellationToken cancellationToken)
    {
        var product = new Product(title, price, discount);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Publish the ProductCreatedEvent using MediatR
        var productCreatedEvent = new IncreaseProductInventoryEvent(product.Id, inventoryCount);
        await _mediator.Publish(productCreatedEvent, cancellationToken);

        return product.Id;
    }

    public async Task<ProductDto> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { id },
        cancellationToken: cancellationToken)
                                                    ?? throw new EntityNotFoundException<Product>(id);
        return new ProductDto
        (
            Id: product.Id,
            Title: product.Title,
            InventoryCount: product.InventoryCount,
            Price: product.Price,
            Discount: product.Discount
        );
    }

    public async Task UpdateProductAsync(Guid id, UpdateProductDto productDto, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object?[] { id },
        cancellationToken: cancellationToken)
                                                    ?? throw new EntityNotFoundException<Product>(id);

        var productCreatedEvent = new UpdateProductEvent(product.Id, productDto.Title, productDto.Price, productDto.Discount);
        await _mediator.Publish(productCreatedEvent, cancellationToken);
    }
}
