using MediatR;

namespace Smeti.Domain.Common;

public interface IError : INotification
{
    DateTimeOffset Timestamp { get; }
}