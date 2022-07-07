using LanguageExt;

namespace Smeti.Domain.Models.ItemModel;

public interface IItemEvent
{
    ItemId ItemId { get; }
    DateTimeOffset Timestamp { get; }
}

public sealed record ItemCreatedEvent(ItemId ItemId, Lst<IItemField> Fields, DateTimeOffset Timestamp) : IItemEvent;

public sealed record FieldAddedEvent(ItemId ItemId, IItemField Field, DateTimeOffset Timestamp) : IItemEvent;

public sealed record FieldUpdatedEvent(ItemId ItemId, IItemField Field, DateTimeOffset Timestamp) : IItemEvent;