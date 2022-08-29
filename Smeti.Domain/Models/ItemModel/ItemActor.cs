using Akka.Actor;
using Akka.Hosting;
using Akka.Persistence;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using Smeti.Domain.Common;
using Smeti.Domain.Common.Errors;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel.Utils;
using Smeti.Domain.Common.Extensions;

namespace Smeti.Domain.Models.ItemModel;

public sealed class ItemActor : ReceivePersistentActor
{
    private readonly IReadOnlyActorRegistry _actorRegistry;
    private Option<ItemActorState> _state;

    [UsedImplicitly]
    public ItemActor(string persistenceId, IReadOnlyActorRegistry actorRegistry)
    {
        _actorRegistry = actorRegistry;
        PersistenceId = persistenceId;
        _state = Prelude.None;
        Recover<IItemEvent>(ApplyEvent);
        Recover<SnapshotOffer>(ApplySnapshot);
        Command<GetItemCommand>(HandleCommand);
        CommandAsync<CreateItemCommand>(HandleCommand);
        CommandAsync<AddFieldCommand>(HandleCommand);
        Command<RemoveFieldCommand>(HandleCommand);
        Command<DeleteItemCommand>(HandleCommand);
    }

    public override string PersistenceId { get; }

    private void HandleCommand(GetItemCommand command)
    {
        Reply(command.ItemId);
    }

    private async Task HandleCommand(CreateItemCommand command)
    {
        var (itemId, itemDefinitionId, fields) = command;
        switch(_state.Case)
        {
            case ItemActorState _:
                Reply(ItemError.ItemAlreadyExist(itemId));
                break;
            case null:
                var fieldDuplicates = fields.Select(f => f.FieldName).FindDuplicates();
                if(fieldDuplicates.IsEmpty == false)
                {
                    Reply(ItemError.FieldDuplicates(itemId, fieldDuplicates));
                    break;
                }

                var errorOrItemDef = await GetItemDefinition(itemDefinitionId);
                var fieldsOrError = errorOrItemDef
                   .Bind(definition =>
                    {
                        var defMap = definition.FieldDefinitions.Select(fd => (fd.FieldName, fd))
                                               .ToMap();
                        return VerifyFields(fields, defMap)
                              .Traverse()
                              .MapLeft(errors => ItemError.InvalidFields(itemId, definition.Id, errors));
                    });
                switch(fieldsOrError.Case)
                {
                    case IDomainError error:
                        Reply(error);
                        break;
                    case Lst<IField> verifiedFields:
                        var @event = new ItemCreatedEvent(itemId, itemDefinitionId, verifiedFields, DateTimeOffset.Now);
                        // ReSharper disable once MethodHasAsyncOverload
                        Persist(@event, OnEventPersisted);
                        break;
                }

                break;
        }
    }

    private async Task HandleCommand(AddFieldCommand command)
    {
        var (itemId, field) = command;
        switch(_state.Case)
        {
            case null:
                Reply(ItemError.ItemNotExist(itemId));
                break;
            case ItemActorState { IsDeleted: true }:
                Reply(ItemError.Deleted(itemId));
                break;
            case ItemActorState { Fields: var fields } when fields.ContainsKey(field.FieldName):
                Reply(ItemError.AlreadyHasField(itemId, field.FieldName));
                break;
            case ItemActorState { ItemDefinitionId: var definitionId }:
                var errorOrDefinition = await GetItemDefinition(definitionId);
                var fieldsOrError = errorOrDefinition
                   .Bind(definition =>
                    {
                        var defMap = definition.FieldDefinitions.Select(fd => (fd.FieldName, fd))
                                               .ToMap();
                        return VerifyFields(List.create(field), defMap)
                              .Traverse()
                              .MapLeft(errors => ItemError.InvalidFields(itemId, definition.Id, errors));
                    });
                switch(fieldsOrError.Case)
                {
                    case IDomainError error:
                        Reply(error);
                        break;
                    default:
                        var @event = new FieldAddedEvent(itemId, field, DateTimeOffset.Now);
                        // ReSharper disable once MethodHasAsyncOverload
                        Persist(@event, OnEventPersisted);
                        break;
                }

                break;
        }
    }

    private void HandleCommand(RemoveFieldCommand command)
    {
        var (itemId, fieldName) = command;
        switch(_state.Case)
        {
            case null:
                Reply(ItemError.ItemNotExist(itemId));
                break;
            case ItemActorState { IsDeleted: true }:
                Reply(ItemError.Deleted(itemId));
                break;
            case ItemActorState { Fields: var fields } when fields.ContainsKey(fieldName) == false:
                Reply(ItemError.DoesNotHaveField(itemId, fieldName));
                break;
            default:
                var @event = new FieldRemovedEvent(itemId, fieldName, DateTimeOffset.Now);
                Persist(@event, OnEventPersisted);
                break;
        }
    }

    private void HandleCommand(DeleteItemCommand command)
    {
        switch(_state.Case)
        {
            case null:
                Reply(ItemError.ItemNotExist(command.ItemId));
                break;
            case ItemActorState { IsDeleted: true }:
                Reply(ItemError.Deleted(command.ItemId));
                break;
            default:
                var @event = new ItemDeletedEvent(command.ItemId, DateTimeOffset.Now);
                Persist(@event, OnEventPersisted);
                break;
        }
    }

    private async Task<Either<IDomainError, ItemDefinition>> GetItemDefinition(ItemDefinitionId itemDefinitionId)
    {
        var itemDefActor = _actorRegistry.Get<ItemDefinitionActor>();
        var getItemDefCommand = new GetItemDefinitionCommand(itemDefinitionId);
        var errorOrItemDef = await itemDefActor.Ask<Either<IDomainError, ItemDefinition>>(
                                 getItemDefCommand,
                                 0.5.Seconds()
                             );
        return errorOrItemDef;
    }

    private static IEnumerable<Either<(FieldName, InvalidFieldReason), IField>> VerifyFields(
        IEnumerable<IField> fields,
        Map<FieldName, FieldDefinition> fieldDefinitions
    )
    {
        foreach(var field in fields)
        {
            var definition = fieldDefinitions.Find(field.FieldName);
            switch(definition.Case)
            {
                case null:
                    yield return Prelude.Left<(FieldName, InvalidFieldReason), IField>((
                        field.FieldName,
                        InvalidFieldReason.NotRegistered));
                    break;
                case FieldDefinition fd:
                    yield return fd.ValueType switch
                    {
                        FieldValueType.Boolean when field is IField<bool> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        FieldValueType.Integer when field is IField<long> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        FieldValueType.Double when field is IField<double> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        FieldValueType.Decimal when field is IField<decimal> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        FieldValueType.DateTime when field is IField<DateTimeOffset> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        FieldValueType.TimeSpan when field is IField<TimeSpan> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        FieldValueType.Reference when field is IField<ItemId> =>
                            Prelude.Right<(FieldName, InvalidFieldReason), IField>(field),
                        _ => Prelude.Left<(FieldName, InvalidFieldReason), IField>(
                            (field.FieldName, InvalidFieldReason.InvalidValueType))
                    };
                    break;
            }
        }
    }

    private void OnEventPersisted(IItemEvent @event)
    {
        ApplyEvent(@event);
        SaveSnapshot();
        Reply(@event.ItemId);
    }

    private void ApplyEvent(IItemEvent @event)
    {
        _state = _state.ApplyEvent(@event);
    }

    private void Reply(ItemId itemId)
    {
        Sender.Tell(_state.Map(s => s.ToItem(itemId)).ToEither(ItemError.ItemNotExist(itemId)));
    }

    private void Reply(IDomainError error)
    {
        Sender.Tell(Prelude.Left<IDomainError, Item>(error));
    }

    private void SaveSnapshot()
    {
        if(LastSequenceNr % 500 == 0)
        {
            _state.Iter(SaveSnapshot);
        }
    }

    private void ApplySnapshot(SnapshotOffer offer)
    {
        if(offer.Snapshot is ItemActorState state)
        {
            _state = state;
        }
    }
}