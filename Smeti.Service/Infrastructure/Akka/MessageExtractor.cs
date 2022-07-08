using Akka.Cluster.Sharding;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Service.Infrastructure.Akka;

public sealed class MessageExtractor : IMessageExtractor
{
    public string EntityId(object message) => message switch
    {
        IItemCommand command => $"item-{command.ItemId}",
        _                    => throw new ArgumentException("Not supported message", nameof(message))
    };

    public object EntityMessage(object message) => message switch
    {
        IItemCommand command => command,
        _                    => throw new ArgumentException("Not supported message", nameof(message))
    };

    public string ShardId(object message) => message switch
    {
        IItemCommand => KnownShards.Item,
        _            => throw new ArgumentException("Not supported message", nameof(message))
    };
}