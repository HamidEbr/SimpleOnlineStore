using Application.Queries;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace SimpleOnlineStore.Test;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseInMemoryDatabase(databaseName: "GetProductByIdQueryHandler_ShouldReturnProduct")
            .Options;

        using var dbContext = new StoreContext(options);
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

        var product = new Product
        {
            Title = "Product 1",
            InventoryCount = 10,
            Price = 100,
            Discount = 10
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        var handler = new GetProductByIdQuery.Handler(dbContext, cache);

        var query = new GetProductByIdQuery(Id: product.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Title, result.Title);
        Assert.Equal(product.InventoryCount, result.InventoryCount);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(product.Discount, result.Discount);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseInMemoryDatabase(databaseName: "GetProductByIdQueryHandler_ShouldReturnNull")
            .Options;

        using var dbContext = new StoreContext(options);
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

        var handler = new GetProductByIdQuery.Handler(dbContext, cache);

        var query = new GetProductByIdQuery(Id: Guid.NewGuid()); // use a random id that doesn't exist in the database

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
