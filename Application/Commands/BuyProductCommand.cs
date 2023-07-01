using Application.Services;
using FluentValidation;
using MediatR;

namespace Application.Commands;

public sealed record BuyProductCommand(Guid ProductId, Guid UserId, int Quantity) : IRequest
{
    public class Validator : AbstractValidator<BuyProductCommand>
    {
        public Validator()
        {
            RuleFor(v => v.ProductId).NotEmpty();
            RuleFor(v => v.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<BuyProductCommand>
    {
        readonly IOrderService _orderService;

        public Handler(IOrderService orderService) => _orderService = orderService;

        public Task Handle(BuyProductCommand request, CancellationToken cancellationToken)
            => _orderService.AddOrderAsync(request.ProductId, request.UserId, request.Quantity, cancellationToken);
    }
}