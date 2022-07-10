using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Smeti.Projection;

namespace Smeti.Service.Services.Projections;

public sealed class EventQueryProvider : IEventQueryProvider
{
    private readonly ActorSystem _actorSystem;

    public EventQueryProvider(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;
    }

    public IAllEventsQuery EventsQuery => PersistenceQuery
                                         .Get(_actorSystem)
                                         .ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
}