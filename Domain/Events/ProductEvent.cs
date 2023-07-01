using MediatR;

namespace Domain.Events;

// Base class for product events
public abstract record ProductEvent(Guid ProductId) : INotification
{
    public DateTime EventDate { get; set; } = DateTime.UtcNow;
}
