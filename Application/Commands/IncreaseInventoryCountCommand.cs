﻿using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure.Persistance;
using MediatR;

namespace Application.Commands;

public sealed record IncreaseInventoryCountCommand(Guid ProductId, int Count) : IRequest
{
    public class Validator : AbstractValidator<IncreaseInventoryCountCommand>
    {
        public Validator()
        {
            RuleFor(v => v.ProductId).NotEmpty();
            RuleFor(v => v.Count).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<IncreaseInventoryCountCommand>
    {
        private readonly StoreContext _dbContext;

        public Handler(StoreContext dbContext) => _dbContext = dbContext;

        public async Task Handle(IncreaseInventoryCountCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(
                new object?[] { request.ProductId },
                cancellationToken: cancellationToken) ?? throw new EntityNotFoundException<Product>(request.ProductId);
            product.ApplyEvent(new IncreaseProductInventoryEvent(request.ProductId, request.Count));
        }
    }
}
