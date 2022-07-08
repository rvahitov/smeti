using LanguageExt;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IItemDefinitionEvent
{
    ItemDefinitionId ItemDefinitionId { get; }
}

public sealed record ItemDefinitionCreatedEvent(
    ItemDefinitionId ItemDefinitionId,
    Option<ItemDefinitionTitle> ItemDefinitionTitle,
    Lst<IFieldDefinition> FieldDefinitions = default
) : IItemDefinitionEvent;

public sealed record FieldDefinitionAddedEvent(ItemDefinitionId ItemDefinitionId, IFieldDefinition FieldDefinition)
    : IItemDefinitionEvent;

public sealed record FieldDefinitionUpdatedEvent(ItemDefinitionId ItemDefinitionId, IFieldDefinition FieldDefinition)
    : IItemDefinitionEvent;