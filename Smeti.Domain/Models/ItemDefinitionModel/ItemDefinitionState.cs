using LanguageExt;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public sealed record ItemDefinitionState(
    Option<ItemDefinitionTitle> Title,
    Map<FieldName, IFieldDefinition> FieldDefinitions
)
{
    public ItemDefinition ToModel(ItemDefinitionId id) => new(id, Title, FieldDefinitions.Values.Freeze());
}

public static class ItemDefinitionStateExtensions
{
    public static Option<ItemDefinitionState> ApplyEvent(
        this Option<ItemDefinitionState> state,
        IItemDefinitionEvent @event
    ) => @event switch
    {
        ItemDefinitionCreatedEvent(_, var title, var definitions) =>
            new ItemDefinitionState(title, Map.createRange(definitions.Select(d => (d.FieldName, d)))),
        FieldDefinitionAddedEvent(_, var definition) =>
            state.Map(s => s with { FieldDefinitions = s.FieldDefinitions.Add(definition.FieldName, definition) }),
        FieldDefinitionUpdatedEvent(_, var definition) =>
            state.Map(s => s with { FieldDefinitions = s.FieldDefinitions.SetItem(definition.FieldName, definition) }),
        _ => state
    };
}