using Akka.Actor;
using Akka.Persistence;
using LanguageExt;
using LanguageExt.Common;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemModel;

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
        switch(command)
        {
            case GetItemDefinitionCommand(var id):
                Sender.Tell(_state.Map(s => s.ToModel(id)).ToEither(ItemDefinitionError.NotExist(id)));
                return;
            case ValidateItemFieldsCommand validate:
                HandleCommand(validate);
                return;
            default:
            {
                var errorOrEvent = TryCreateEventFromCommand(command);
                Prelude.match(errorOrEvent,
                    @event => Persist(@event, OnEventPersisted),
                    error => Sender.Tell(Prelude.Left<Error, ItemDefinition>(error))
                );
                break;
            }
        }
    }

    private void HandleCommand(ValidateItemFieldsCommand command)
    {
        var (id, itemFields) = command;
        if(_state.IsNone)
        {
            Sender.Tell(Prelude.Left<Error, ItemDefinition>(ItemDefinitionError.NotExist(id)));
            return;
        }

        foreach(var itemField in itemFields.Where(itemField =>
                    _state.Map(s => s.FieldDefinitions.ContainsKey(itemField.FieldName)).IfNone(false) == false))
        {
            Sender.Tell(
                Prelude.Left<Error, ItemDefinition>(
                    ItemDefinitionError.NotHaveFieldDefinition(id, itemField.FieldName)
                )
            );
            return;
        }

        var response =
            itemFields
               .Select(Validate)
               .SelectMany(v => v.FailAsEnumerable())
               .Freeze()
               .Apply(
                    validationErrors => validationErrors.Count == 0
                                            ? _state.Map(s => s.ToModel(id)).ToEither(ItemDefinitionError.NotExist(id))
                                            : Prelude.Left<Error, ItemDefinition>(
                                                ItemDefinitionError.InvalidFieldValue(id, validationErrors))
                );
        Sender.Tell(response);
    }

    private Validation<string, IItemField> Validate(IItemField itemField)
    {
        var specifications = _state.Bind(s => s.FieldDefinitions.Find(itemField.FieldName))
                                   .SelectMany(fd => fd.GetSpecifications())
                                   .OrderBy(spec => spec.Order);
        foreach(var spec in specifications)
        {
            var validation = spec.ValidateFieldValue(itemField.FieldName, itemField.GetValue());
            if(validation.IsFail) return validation.Map(_ => itemField);
        }

        return Prelude.Success<string, IItemField>(itemField);
    }

    private Either<Error, IItemDefinitionEvent> TryCreateEventFromCommand(IItemDefinitionCommand command)
    {
        switch(command)
        {
            case CreateItemDefinitionCommand when _state.IsSome:
                return ItemDefinitionError.AlreadyExists(command.ItemDefinitionId);

            case CreateItemDefinitionCommand(var id, var title, var definitions):
                var duplicates = FindDuplicates(definitions);
                if(duplicates.Count != 0)
                {
                    return ItemDefinitionError.FieldDefinitionsDuplicates(id, duplicates);
                }

                return new ItemDefinitionCreatedEvent(id, title, definitions);

            case AddFieldDefinitionCommand when _state.IsNone:
                return ItemDefinitionError.NotExist(command.ItemDefinitionId);

            case AddFieldDefinitionCommand(var id, var definition):
                return _state
                      .Map(s => s.FieldDefinitions.ContainsKey(definition.FieldName)).IfNone(true)
                           ? ItemDefinitionError.AlreadyHasFieldDefinition(id, definition.FieldName)
                           : new FieldDefinitionAddedEvent(id, definition);

            case UpdateFieldDefinitionCommand when _state.IsNone:
                return ItemDefinitionError.NotExist(command.ItemDefinitionId);

            case UpdateFieldDefinitionCommand(var id, var definition):
                return _state
                      .Map(s => s.FieldDefinitions.ContainsKey(definition.FieldName)).IfNone(false)
                           ? new FieldDefinitionUpdatedEvent(id, definition)
                           : ItemDefinitionError.NotHaveFieldDefinition(id, definition.FieldName);
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
        Sender.Tell(_state.Map(s => s.ToModel(id)).ToEither(ItemDefinitionError.NotExist(id)));
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