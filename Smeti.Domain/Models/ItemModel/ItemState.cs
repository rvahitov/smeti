using LanguageExt;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemModel;

public sealed record ItemState(Map<FieldName, IItemField> Fields)
{
    public Item ToModel(ItemId id) => new(id, Fields.Values.Freeze());
}

public static class ItemStateExtensions
{
    public static Option<ItemState> ApplyEvent(this Option<ItemState> state, IItemEvent @event) => @event switch
    {
        ItemCreatedEvent(_, var fields, _) when state.IsNone =>
            new ItemState(Map.createRange(fields.Select(f => (f.FieldName, f)))),

        FieldAddedEvent(_, var field, _) =>
            state.Map(s => new ItemState(s.Fields.Add(field.FieldName, field))),
        FieldUpdatedEvent(_, var field, _) =>
            state.Map(s => new ItemState(s.Fields.SetItem(field.FieldName, field))),
        _ => state
    };
}