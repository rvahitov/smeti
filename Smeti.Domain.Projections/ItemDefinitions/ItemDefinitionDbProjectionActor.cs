using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Projections.Services;
using Smeti.Projection;

namespace Smeti.Domain.Projections.ItemDefinitions;

public sealed class ItemDefinitionDbProjectionActor : BaseProjectionActor<IItemDefinitionEvent>
{
    private readonly IPostgresConnectionFactory _connectionFactory;

    public ItemDefinitionDbProjectionActor(
        IEventQueryProvider queryProvider,
        IPostgresConnectionFactory connectionFactory
    ) : base(queryProvider)
    {
        _connectionFactory = connectionFactory;
        PersistenceId = typeof(ItemDefinitionDbProjectionActor).FullName!;
    }

    public override string PersistenceId { get; }

    protected override void ProcessEvents(IEnumerable<IItemDefinitionEvent> events)
    {
        ItemDefinitionProjectionRepository.WriteEvents(_connectionFactory, events);
    }
}