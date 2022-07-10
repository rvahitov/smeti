using Akka.Persistence.Query;

namespace Smeti.Projection;

public interface IEventQueryProvider
{
    IAllEventsQuery EventsQuery { get; }
}