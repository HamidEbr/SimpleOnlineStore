using Api.Behaviors;
using Application;
using Application.Commands;
using Application.Models;
using Application.Queries;
using Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlDbContext(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddApplication(typeof(CachingBehavior<,>), typeof(ValidationBehavior<,>), typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/products", async (CreateProductCommand command, IMediator mediator) =>
{
    var productId = await mediator.Send(command);
    return Results.Created($"/api/products/{productId}", new { id = productId });
    //return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, result);
});

app.MapPut("/api/products/{id}/inventory", async (Guid id, int count, IMediator mediator) =>
{
    await mediator.Send(new IncreaseInventoryCountCommand(ProductId: id, Count: count));
    return Results.NoContent();
});

app.MapGet("/api/products/{id}", async (Guid id, IMediator mediator) =>
{
    var productDto = await mediator.Send(new GetProductByIdQuery(Id: id));
    return Results.Ok(productDto);
});

app.MapPost("/api/users/{userId}/orders", async (Guid userId, OrderDto orderDto, IMediator mediator) =>
{
    await mediator.Send(new BuyProductCommand(ProductId: orderDto.ProductId, UserId: userId, 1));
    return Results.NoContent();
});

app.MapPut("/products/{id}", async (int id, UpdateProductCommand command, IMediator mediator) =>
{
    await mediator.Send(command);
    return Results.NoContent();
});

app.Run();