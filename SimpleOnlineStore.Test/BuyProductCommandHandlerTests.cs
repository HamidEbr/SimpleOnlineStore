﻿using Application.Commands;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace SimpleOnlineStore.Test;

public class BuyProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldBuyProduct_WhenInventoryCountIsGreaterThanZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseInMemoryDatabase(databaseName: "BuyProductCommandHandler_ShouldBuyProduct")
            .Options;

        using var dbContext = new StoreContext(options);
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

        var user = new User
        {
            Name = "User 1",
            Orders = new List<Order>()
        };

        dbContext.Users.Add(user);

        var product = new Product
        {
            Title = "Product 1",
            InventoryCount = 1,
            Price = 100,
            Discount = 10
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        var handler = new BuyProductCommand.Handler(dbContext, cache);

        var command = new BuyProductCommand
        (
            ProductId: product.Id,
            UserId: user.Id
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await dbContext.Products.FindAsync(product.Id);
        var updatedUser = await dbContext.Users.FindAsync(user.Id);

        Assert.NotNull(updatedProduct);
        Assert.Equal(product.InventoryCount - 1, updatedProduct.InventoryCount);

        Assert.NotNull(updatedUser);
        Assert.Single(updatedUser.Orders);
        Assert.Equal(product.Id, updatedUser.Orders[0].Product.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenInventoryCountIsZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseInMemoryDatabase(databaseName: "BuyProductCommandHandler_ShouldThrowException")
            .Options;

        using var dbContext = new StoreContext(options);
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

        var user = new User
        {
            Name = "User 1",
            Orders = new List<Order>()
        };

        dbContext.Users.Add(user);

        var product = new Product
        {
            Title = "Product 1",
            InventoryCount = 0,
            Price = 100,
            Discount = 10
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        var handler = new BuyProductCommand.Handler(dbContext, cache);

        var command = new BuyProductCommand
        (
            ProductId: product.Id,
            UserId: user.Id
        );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}
