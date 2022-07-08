using Akka.Actor;
using Akka.Persistence;
using LanguageExt;
using LanguageExt.Common;
using Smeti.Domain.Models.Common;
using Errors = Smeti.Domain.Models.ItemDefinitionModel.ItemDefinitionErrors;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public sealed class ItemDefinitionActor : ReceivePersistentActor
{
    private Option<ItemDefinitionState> _state;

    public ItemDefinitionActor(string persistenceId)
    {
        PersistenceId = persistenceId;

        Command<IItemDefinitionCommand>(HandleCommand);

        Recover<IItemDefinitionEvent>(ApplyEvent);

        Recover<SnapshotOffer>(offer =>
        {
            if(offer.Snapshot is ItemDefinitionState state)
                _state = state;
        });
    }

    public override string PersistenceId { get; }

    private void HandleCommand(IItemDefinitionCommand command)
    {
        if(command is GetItemDefinitionCommand(var id))
        {
            Sender.Tell(_state.Map(s => s.ToModel(id)).ToEither(Errors.NotExist(id)));
            return;
        }

        var errorOrEvent = TryCreateEventFromCommand(command);
        Prelude.match(errorOrEvent,
            @event => Persist(@event, OnEventPersisted),
            error => Sender.Tell(Prelude.Left<Error, ItemDefinition>(error))
        );
    }

    private Either<Error, IItemDefinitionEvent> TryCreateEventFromCommand(IItemDefinitionCommand command)
    {
        switch(command)
        {
            case CreateItemDefinitionCommand when _state.IsSome:
                return Errors.AlreadyExists(command.ItemDefinitionId);

            case CreateItemDefinitionCommand(var id, var title, var definitions):
                var duplicates = FindDuplicates(definitions);
                if(duplicates.Count != 0)
                {
                    return Errors.FieldDefinitionsDuplicates(id, duplicates);
                }

                return new ItemDefinitionCreatedEvent(id, title, definitions);

            case AddFieldDefinitionCommand when _state.IsNone:
                return Errors.NotExist(command.ItemDefinitionId);

            case AddFieldDefinitionCommand(var id, var definition):
                return _state
                      .Map(s => s.FieldDefinitions.ContainsKey(definition.FieldName)).IfNone(true)
                           ? Errors.AlreadyHasFieldDefinition(id, definition.FieldName)
                           : new FieldDefinitionAddedEvent(id, definition);

            case UpdateFieldDefinitionCommand when _state.IsNone:
                return Errors.NotExist(command.ItemDefinitionId);

            case UpdateFieldDefinitionCommand(var id, var definition):
                return _state
                      .Map(s => s.FieldDefinitions.ContainsKey(definition.FieldName)).IfNone(false)
                           ? new FieldDefinitionUpdatedEvent(id, definition)
                           : Errors.NotHaveFieldDefinition(id, definition.FieldName);
            default:
                return CommonErrors.CommandUnknown(command.GetType().FullName!);
        }
    }

    private void ApplyEvent(IItemDefinitionEvent @event)
    {
        _state = _state.ApplyEvent(@event);
    }

    private void OnEventPersisted(IItemDefinitionEvent @event)
    {
        ApplyEvent(@event);
        var id = @event.ItemDefinitionId;
        Sender.Tell(_state.Map(s => s.ToModel(id)).ToEither(ItemDefinitionErrors.NotExist(id)));
        _state.Filter(_ => LastSequenceNr % 500 == 0).Iter(SaveSnapshot);
    }

    private static Lst<FieldName> FindDuplicates(IEnumerable<IFieldDefinition> fields) =>
        fields
           .Select(f => f.FieldName)
           .Aggregate(
                Map.empty<FieldName, int>(),
                (map, fieldName) => map.AddOrUpdate(fieldName, count => count + 1, () => 1)
            )
           .Filter(count => count > 1)
           .Apply(map => map.Keys.Freeze());
}