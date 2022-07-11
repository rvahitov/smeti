using Smeti.Domain.Models.ItemModel;
using Smeti.Domain.Projections.Services;
using Smeti.Projection;

namespace Smeti.Domain.Projections.Items;

public sealed class ItemDbProjectionActor : BaseProjectionActor<IItemEvent>
{
    private readonly IPostgresConnectionFactory _connectionFactory;

    public ItemDbProjectionActor(
        IEventQueryProvider queryProvider,
        IPostgresConnectionFactory connectionFactory
    ) : base(queryProvider)
    {
        _connectionFactory = connectionFactory;
        PersistenceId = typeof(ItemDbProjectionActor).FullName!;
    }

    public override string PersistenceId { get; }

    protected override void ProcessEvents(IEnumerable<IItemEvent> events)
    {
        ItemsProjectionRepository.WriteEvents(_connectionFactory, events);
    }
}