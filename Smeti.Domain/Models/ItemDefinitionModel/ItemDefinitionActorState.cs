using LanguageExt;
using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public sealed record ItemDefinitionActorState(
    ItemDefinitionName ItemDefinitionName,
    Lst<FieldDefinition> FieldDefinitions
)
{
    public ItemDefinition ToItemDefinition(ItemDefinitionId id) =>
        new(id, ItemDefinitionName, FieldDefinitions.Freeze());

    public bool ContainsField(FieldName fieldName) =>
        FieldDefinitions.Find(fd => fd.FieldName == fieldName)
            ? true
            : false;
}

public static class ItemDefinitionActorStateHelps
{
    public static Option<ItemDefinitionActorState> ApplyEvent(
        this Option<ItemDefinitionActorState> state,
        IItemDefinitionEvent @event
    ) => @event switch
    {
        ItemDefinitionCreatedEvent(_, var name, _) when state.IsNone =>
            new ItemDefinitionActorState(name, List.empty<FieldDefinition>()),
        FieldDefinitionAddedEvent(_, var fieldDefinition, _) =>
            state.Map(s => s with { FieldDefinitions = s.FieldDefinitions.Add(fieldDefinition) }),
        FieldDefinitionRemovedEvent(_, var fieldName, _) =>
            state.Map(s => s with { FieldDefinitions = s.FieldDefinitions.Filter(fd => fd.FieldName != fieldName) }),
        _ => state
    };
}