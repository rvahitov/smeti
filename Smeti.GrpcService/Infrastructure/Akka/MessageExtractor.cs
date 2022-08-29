using Akka.Cluster.Sharding;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Infrastructure.Akka;

public sealed class MessageExtractor : IMessageExtractor
{
    public string EntityId(object message) => message switch
    {
        IItemDefinitionCommand { ItemDefinitionId: (var id) _ } => $"item_definition_{id}",
        IItemCommand { ItemId: (var id) _ } => $"item_{id}",
        _                                   => throw new ArgumentException("Not supported message", nameof(message))
    };

    public object EntityMessage(object message) => message switch
    {
        IItemDefinitionCommand command => command,
        IItemCommand command           => command,
        _                              => throw new ArgumentException("Not supported message", nameof(message))
    };

    public string ShardId(object message) => message switch
    {
        IItemDefinitionCommand _ => KnownShards.ItemDefinition,
        IItemCommand _           => KnownShards.Item,
        _                        => throw new ArgumentException("Not supported message", nameof(message))
    };
}