using Akka.Actor;
using Akka.Persistence;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Common;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemModel;

public sealed class ItemActor : ReceivePersistentActor
{
    private Option<ItemState> _state;

    [UsedImplicitly]
    public ItemActor(string persistenceId)
    {
        PersistenceId = persistenceId;

        Command<IItemCommand>(command =>
        {
            if(command is GetItemCommand(var id))
            {
                Sender.Tell(_state.Map(s => s.ToModel(id)).ToEither(ItemErrors.NotExist(id)));
                return;
            }

            var errorOrEvent = TryCreateEventFromCommand(command);
            Prelude.match(
                errorOrEvent,
                @event => Persist(@event, OnEventPersisted),
                error => Sender.Tell(Prelude.Left<Error, Item>(error))
            );
        });

        Recover<IItemEvent>(@event => _state = _state.ApplyEvent(@event));

        Recover<SnapshotOffer>(offer =>
        {
            if(offer.Snapshot is ItemState state)
            {
                _state = state;
            }
        });
    }

    public override string PersistenceId { get; }

    private void OnEventPersisted(IItemEvent @event)
    {
        _state = _state.ApplyEvent(@event);
        var itemId = @event.ItemId;
        Sender.Tell(_state.Map(s => s.ToModel(itemId)).ToEither(ItemErrors.NotExist(itemId)));
        _state.IfSome(s =>
        {
            if(LastSequenceNr % 500 == 0)
                SaveSnapshot(s);
        });
    }

    private Either<Error, IItemEvent> TryCreateEventFromCommand(IItemCommand command)
    {
        switch(command)
        {
            case CreateItemCommand when _state.IsSome:
                return ItemErrors.AlreadyExists(command.ItemId);

            case CreateItemCommand(var id, var fields):
                var duplicates = FindDuplicates(fields);
                if(duplicates.Count != 0)
                {
                    return ItemErrors.FieldsDuplicates(id, duplicates);
                }

                return new ItemCreatedEvent(id, fields, DateTimeOffset.Now);

            case AddFieldCommand when _state.IsNone:
                return ItemErrors.NotExist(command.ItemId);

            case AddFieldCommand(var id, var field):
                return _state.Map(s => s.Fields.ContainsKey(field.FieldName)).IfNone(false)
                           ? ItemErrors.AlreadyHasField(id, field.FieldName)
                           : new FieldAddedEvent(id, field, DateTimeOffset.Now);

            case UpdateFieldCommand when _state.IsNone:
                return ItemErrors.NotExist(command.ItemId);

            case UpdateFieldCommand(var id, var field):
                return _state.Map(s => s.Fields.ContainsKey(field.FieldName)).IfNone(false)
                           ? new FieldUpdatedEvent(id, field, DateTimeOffset.Now)
                           : ItemErrors.NotHaveField(id, field.FieldName);

            default:
                return CommonErrors.CommandUnknown(command.GetType().FullName!);
        }
    }

    private static Lst<FieldName> FindDuplicates(IEnumerable<IItemField> fields) =>
        fields
           .Select(f => f.FieldName)
           .Aggregate(
                Map.empty<FieldName, int>(),
                (map, fieldName) => map.AddOrUpdate(fieldName, count => count + 1, () => 1)
            )
           .Filter(count => count > 1)
           .Apply(map => map.Keys.Freeze());
}