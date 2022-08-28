using LanguageExt;
using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public sealed record ItemDefinitionActorState(
    ItemDefinitionName ItemDefinitionName,
    Map<FieldName, FieldDefinition> FieldDefinitions
)
{
    public ItemDefinition ToItemDefinition(ItemDefinitionId id) =>
        new(id, ItemDefinitionName, FieldDefinitions.Values.Freeze());
}

public static class ItemDefinitionActorStateHelps
{
    public static Option<ItemDefinitionActorState> ApplyEvent(
        this Option<ItemDefinitionActorState> state,
        IItemDefinitionEvent @event
    ) => @event switch
    {
        ItemDefinitionCreatedEvent(_, var name, _) when state.IsNone =>
            new ItemDefinitionActorState(name, Map.empty<FieldName, FieldDefinition>()),
        FieldDefinitionAddedEvent(_, (var fieldName, _) fieldDefinition, _) =>
            state.Map(s => s with { FieldDefinitions = s.FieldDefinitions.Add(fieldName, fieldDefinition) }),
        FieldDefinitionRemovedEvent(_, var fieldName, _) =>
            state.Map(s => s with { FieldDefinitions = s.FieldDefinitions.Remove(fieldName) }),
        _ => state
    };
}