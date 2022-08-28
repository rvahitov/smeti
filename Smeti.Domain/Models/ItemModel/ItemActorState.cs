// ReSharper disable WithExpressionModifiesAllMembers

using LanguageExt;
using Smeti.Domain.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

internal sealed record ItemActorState(ItemDefinitionId ItemDefinitionId, Map<FieldName, IField> Fields, bool IsDeleted)
{
    public Item ToItem(ItemId id) => new(id, ItemDefinitionId, Fields.Values.Freeze(), IsDeleted);
}

internal static class ItemActorStateHelps
{
    public static Option<ItemActorState> ApplyEvent(this Option<ItemActorState> state, IItemEvent @event) =>
        @event switch
        {
            ItemCreatedEvent(_, var itemDefinitionId, var fields, _) when state.IsNone =>
                new ItemActorState(itemDefinitionId, fields.Select(f => (f.FieldName, f)).ToMap(), false),
            FieldAddedEvent(_, var field, _) =>
                state.Map(s => s with { Fields = s.Fields.AddOrUpdate(field.FieldName, field) }),
            FieldRemovedEvent(_, var fieldName, _) =>
                state.Map(s => s with { Fields = s.Fields.Remove(fieldName) }),
            ItemDeletedEvent _ =>
                state.Map(s => s with { Fields = Map.empty<FieldName, IField>(), IsDeleted = true }),
            _ => state
        };
}