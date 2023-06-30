using Application.Commands;
using Application.Models;
using Application.Queries;
using Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

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
});

app.MapPut("/api/products/{id:int}/inventory", async (int id, int count, IMediator mediator) =>
{
    await mediator.Send(new IncreaseInventoryCountCommand(ProductId: id, Count: count));
    return Results.NoContent();
});

app.MapGet("/api/products/{id:int}", async (Guid id, IMediator mediator) =>
{
    var productDto = await mediator.Send(new GetProductByIdQuery(Id: id));
    return Results.Ok(productDto);
});

app.MapPost("/api/users/{userId:int}/orders", async (Guid userId, OrderDto orderDto, IMediator mediator) =>
{
    await mediator.Send(new BuyProductCommand(ProductId: orderDto.ProductId, UserId: userId));
    return Results.NoContent();
});

app.Run();