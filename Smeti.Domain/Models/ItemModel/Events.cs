using LanguageExt;
using Smeti.Domain.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public interface IItemEvent : IDomainEvent
{
    ItemId ItemId { get; }
}

public sealed record ItemCreatedEvent(
    ItemId ItemId,
    ItemDefinitionId ItemDefinitionId,
    Lst<IField> Fields,
    DateTimeOffset Timestamp
) : IItemEvent;

public sealed record FieldAddedEvent(ItemId ItemId, IField Field, DateTimeOffset Timestamp) : IItemEvent;

public sealed record FieldRemovedEvent(ItemId ItemId, FieldName FieldName, DateTimeOffset Timestamp) : IItemEvent;

public sealed record ItemDeletedEvent(ItemId ItemId, DateTimeOffset Timestamp) : IItemEvent;