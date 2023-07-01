using Application.Commands;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SimpleOnlineStore.Test;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddProduct_WhenValidDataIsProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseInMemoryDatabase(databaseName: "AddProductCommandHandler_ShouldAddProduct")
            .Options;

        using var dbContext = new StoreContext(options);

        var handler = new CreateProductCommand.Handler(dbContext, default!);

        var command = new CreateProductCommand
        (
            Title: "Product 1",
            InventoryCount: 10,
            Price: 100,
            Discount: 10
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        var product = await dbContext.Products.FindAsync(result);

        Assert.NotNull(product);
        Assert.Equal(result, product.Id);
        Assert.Equal(command.Title, product.Title);
        Assert.Equal(command.InventoryCount, product.InventoryCount);
        Assert.Equal(command.Price, product.Price);
        Assert.Equal(command.Discount, product.Discount);
    }
}
