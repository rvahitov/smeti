using MediatR;

namespace Smeti.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTimeOffset Timestamp { get; }
}