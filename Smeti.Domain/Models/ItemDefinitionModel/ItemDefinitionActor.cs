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

        Command<CreateItemDefinitionCommand>(HandleCommand);
        Command<AddFieldDefinitionCommand>(HandleCommand);
        Command<UpdateFieldDefinitionCommand>(HandleCommand);
        Command<ValidateItemFieldsCommand>(HandleCommand);
        Command<GetItemDefinitionCommand>(HandleCommand);

        Recover<IItemDefinitionEvent>(ApplyEvent);

        Recover<SnapshotOffer>(offer =>
        {
            if(offer.Snapshot is ItemDefinitionState state)
                _state = state;
        });
    }

    public override string PersistenceId { get; }

    private void HandleCommand(CreateItemDefinitionCommand command)
    {
        var (id, title, fieldDefinitions) = command;
        if(_state.IsSome)
        {
            Sender.Tell(Prelude.Left<Error, ItemDefinition>(ItemDefinitionError.AlreadyExists(id)));
            return;
        }

        var duplicates = FindDuplicates(fieldDefinitions);
        if(duplicates.Count > 0)
        {
            Sender.Tell(
                Prelude.Left<Error, ItemDefinition>(ItemDefinitionError.FieldDefinitionsDuplicates(id, duplicates))
            );
            return;
        }

        Persist(new ItemDefinitionCreatedEvent(id, title, fieldDefinitions), OnEventPersisted);
    }

    private void HandleCommand(AddFieldDefinitionCommand command)
    {
        var (id, fieldDefinition) = command;

        var errorOrEvent =
            GetState(id)
               .Bind(state => state.FieldDefinitions.ContainsKey(fieldDefinition.FieldName)
                                  ? Prelude.Left<Error, IItemDefinitionEvent>(
                                      ItemDefinitionError.AlreadyHasFieldDefinition(id, fieldDefinition.FieldName))
                                  : Prelude.Right<Error, IItemDefinitionEvent>(
                                      new FieldDefinitionAddedEvent(id, fieldDefinition))
                );
        Prelude.match(
            errorOrEvent,
            @event => Persist(@event, OnEventPersisted),
            error => Sender.Tell(Prelude.Left<Error, ItemDefinition>(error))
        );
    }

    private void HandleCommand(UpdateFieldDefinitionCommand command)
    {
        var (id, fieldDefinition) = command;
        var currentDefinition =
            GetState(id)
               .Bind(
                    s => s.FieldDefinitions
                          .Find(fieldDefinition.FieldName)
                          .ToEither(ItemDefinitionError.NotHaveFieldDefinition(id, fieldDefinition.FieldName))
                );
        Prelude.match
        (
            currentDefinition,
            current =>
            {
                if(current == fieldDefinition)
                {
                    Sender.Tell(GetModel(id));
                }
                else
                {
                    var @event = new FieldDefinitionUpdatedEvent(id, fieldDefinition);
                    Persist(@event, OnEventPersisted);
                }
            },
            error => { Sender.Tell(Prelude.Left<Error, ItemDefinition>(error)); }
        );
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

    private void HandleCommand(GetItemDefinitionCommand command)
    {
        Sender.Tell(GetModel(command.ItemDefinitionId));
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

    private Either<Error, ItemDefinitionState> GetState(ItemDefinitionId id) =>
        _state.ToEither(ItemDefinitionError.NotExist(id));

    private Either<Error, ItemDefinition> GetModel(ItemDefinitionId id) => GetState(id).Map(s => s.ToModel(id));
}