using LanguageExt;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public sealed record ItemState(ItemDefinitionId ItemDefinitionId, Map<FieldName, IItemField> Fields)
{
    public Item ToModel(ItemId id) => new(id, ItemDefinitionId, Fields.Values.Freeze());
}

public static class ItemStateExtensions
{
    public static Option<ItemState> ApplyEvent(this Option<ItemState> state, IItemEvent @event) => @event switch
    {
        ItemCreatedEvent(_, var itemDefinitionId, var fields, _) when state.IsNone =>
            new ItemState(itemDefinitionId, Map.createRange(fields.Select(f => (f.FieldName, f)))),

        FieldAddedEvent(_, var field, _) =>
            state.Map(s => s with { Fields = s.Fields.Add(field.FieldName, field) }),
        FieldUpdatedEvent(_, var field, _) =>
            state.Map(s => s with { Fields = s.Fields.SetItem(field.FieldName, field) }),
        _ => state
    };
}