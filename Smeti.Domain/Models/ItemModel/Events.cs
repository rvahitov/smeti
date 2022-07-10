using LanguageExt;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public interface IItemEvent
{
    ItemId ItemId { get; }
    DateTimeOffset Timestamp { get; }
}

public sealed record ItemCreatedEvent(
    ItemId ItemId,
    ItemDefinitionId ItemDefinitionId,
    Lst<IItemField> Fields,
    DateTimeOffset Timestamp
) : IItemEvent;

public sealed record FieldAddedEvent(ItemId ItemId, IItemField Field, DateTimeOffset Timestamp) : IItemEvent;

public sealed record FieldUpdatedEvent(ItemId ItemId, IItemField Field, DateTimeOffset Timestamp) : IItemEvent;