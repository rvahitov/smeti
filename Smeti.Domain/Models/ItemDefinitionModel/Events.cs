using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IItemDefinitionEvent : IError
{
    ItemDefinitionId ItemDefinitionId { get; }
}

public sealed record ItemDefinitionCreatedEvent(
    ItemDefinitionId ItemDefinitionId,
    ItemDefinitionName ItemDefinitionName,
    DateTimeOffset Timestamp
) : IItemDefinitionEvent;

public sealed record FieldDefinitionAddedEvent(
    ItemDefinitionId ItemDefinitionId,
    FieldDefinition FieldDefinition,
    DateTimeOffset Timestamp
) : IItemDefinitionEvent;

public sealed record FieldDefinitionRemovedEvent(
    ItemDefinitionId ItemDefinitionId,
    FieldName FieldName,
    DateTimeOffset Timestamp
) : IItemDefinitionEvent;

public sealed record ItemDefinitionDeletedEvent(
    ItemDefinitionId ItemDefinitionId,
    DateTimeOffset Timestamp
) : IItemDefinitionEvent;