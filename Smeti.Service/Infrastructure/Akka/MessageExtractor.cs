using Akka.Cluster.Sharding;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Service.Infrastructure.Akka;

public sealed class MessageExtractor : IMessageExtractor
{
    public string EntityId(object message) => message switch
    {
        IItemCommand command           => $"item-{command.ItemId}",
        IItemDefinitionCommand command => $"item-definition-{command.ItemDefinitionId}",
        _                              => throw new ArgumentException("Not supported message", nameof(message))
    };

    public object EntityMessage(object message) => message switch
    {
        IItemCommand command           => command,
        IItemDefinitionCommand command => command,
        _                              => throw new ArgumentException("Not supported message", nameof(message))
    };

    public string ShardId(object message) => message switch
    {
        IItemCommand           => KnownShards.Item,
        IItemDefinitionCommand => KnownShards.ItemDefinition,
        _                      => throw new ArgumentException("Not supported message", nameof(message))
    };
}