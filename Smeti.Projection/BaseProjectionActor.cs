using System.Collections.Immutable;
using Akka;
using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace Smeti.Projection;

public abstract class BaseProjectionActor<TEvent> : ReceivePersistentActor
{
    private UniqueKillSwitch? _killSwitch;

    protected BaseProjectionActor(IEventQueryProvider queryProvider)
    {
        QueryProvider = queryProvider;
        LastOffset = Offset.NoOffset();

        Command<ChangeLastOffsetCommand>(HandleCommand);

        Recover<SnapshotOffer>(offer =>
        {
            if(offer.Snapshot is Offset offset)
            {
                LastOffset = offset;
            }
        });

        Recover<RecoveryCompleted>(_ => SubscribeForEvents());
    }

    protected Offset LastOffset { get; private set; }
    protected IEventQueryProvider QueryProvider { get; }

    protected virtual int EventBufferSize => 1_000;
    protected virtual TimeSpan EventBufferTimeout => TimeSpan.FromSeconds(1);

    protected abstract void ProcessEvents(IEnumerable<TEvent> events);

    private void HandleCommand(ChangeLastOffsetCommand command)
    {
        SaveSnapshot(command.Offset);
        LastOffset = command.Offset;
    }

    private void SubscribeForEvents()
    {
        var self = Self;
        var materializer = Context.Materializer();

        var processEnvelopesSink = Sink
                                  .ForEach<ImmutableList<EventEnvelope>>(envelopes => ProcessEvents(envelopes, self))
                                  .MapMaterializedValue(_ => NotUsed.Instance);

        _killSwitch =
            QueryProvider
               .EventsQuery
               .AllEvents(LastOffset)
               .Where(envelope => envelope.Event is TEvent)
               .GroupedWithin(EventBufferSize, EventBufferTimeout)
               .Select(group => group.ToImmutableList())
               .Async()
               .ViaMaterialized(KillSwitches.Single<ImmutableList<EventEnvelope>>(), Keep.Right)
               .To(processEnvelopesSink)
               .Run(materializer);
    }

    private void ProcessEvents(ImmutableList<EventEnvelope> envelopes, IActorRef selfRef)
    {
        if(envelopes.Count == 0) return;
        var events = envelopes.Select(e => e.Event).Cast<TEvent>().ToImmutableList();
        ProcessEvents(events);
        var changeCommand = new ChangeLastOffsetCommand(
            envelopes.Select(e => e.Offset).Max()!
        );
        selfRef.Tell(changeCommand);
    }

    protected override void PostStop()
    {
        _killSwitch?.Shutdown();
    }
}