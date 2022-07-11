using Akka.Actor;
using Akka.Persistence;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnsafeValueAccess;
using MediatR;
using Smeti.Domain.Extensions;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public sealed class ItemActor : ReceivePersistentActor
{
    private readonly IMediator _mediator;
    private Option<ItemState> _state;

    [UsedImplicitly]
    public ItemActor(string persistenceId, IMediator mediator)
    {
        _mediator = mediator;
        PersistenceId = persistenceId;

        Command<GetItemCommand>(HandleCommand);
        CommandAsync<CreateItemCommand>(HandleCommand);
        CommandAsync<AddFieldCommand>(HandleCommand);
        CommandAsync<UpdateFieldCommand>(HandleCommand);
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

    private void HandleCommand(GetItemCommand command)
    {
        Sender.Tell(GetModel(command.ItemId));
    }

    private async Task HandleCommand(CreateItemCommand command)
    {
        var (itemId, itemDefinitionId, itemFields) = command;
        var duplicates = FindDuplicates(itemFields);
        if(duplicates.Count > 0)
        {
            Sender.Tell(Prelude.Left<Error, Item>(ItemError.FieldsDuplicates(itemId, duplicates)));
            return;
        }

        var validateCommand = new ValidateItemFieldsCommand(itemDefinitionId, itemFields);

        var errorOrEvent =
            await _mediator
                 .TrySend(validateCommand)
                 .Map(_ => new ItemCreatedEvent(itemId, itemDefinitionId, itemFields, DateTimeOffset.Now));
        Prelude.match(
            errorOrEvent,
            @event => Persist(@event, OnEventPersisted),
            error => Sender.Tell(Prelude.Left<Error, Item>(error))
        );
    }

    private async Task HandleCommand(AddFieldCommand command)
    {
        var (itemId, itemField) = command;
        var itemDefinitionId = _state.Map(s => s.ItemDefinitionId);
        if(itemDefinitionId.IsNone)
        {
            Sender.Tell(Prelude.Left<Error, Item>(ItemError.NotExist(itemId)));
            return;
        }

        if(_state.Map(s => s.Fields.ContainsKey(itemField.FieldName)).IfNone(false))
        {
            Sender.Tell(Prelude.Left<Error, Item>(ItemError.AlreadyHasField(itemId, itemField.FieldName)));
            return;
        }

        var validateCommand = new ValidateItemFieldsCommand(itemDefinitionId.ValueUnsafe(), List.create(itemField));
        var errorOrEvent =
            await _mediator
                 .TrySend(validateCommand)
                 .Map(_ => new FieldAddedEvent(itemId, itemField, DateTimeOffset.Now));
        Prelude.match(
            errorOrEvent,
            @event => Persist(@event, OnEventPersisted),
            error => Sender.Tell(Prelude.Left<Error, Item>(error))
        );
    }

    private async Task HandleCommand(UpdateFieldCommand command)
    {
        var (itemId, itemField) = command;

        var errorOrEvent =
            await (from state in GetState(itemId).ToAsync()
                   from _1 in state.Fields.Find(itemField.FieldName)
                                   .ToEither(ItemError.NotHaveField(itemId, itemField.FieldName))
                                   .ToAsync()
                   let validateCommand = new ValidateItemFieldsCommand(state.ItemDefinitionId, List.create(itemField))
                   from _2 in _mediator.TrySend(validateCommand)
                   select new FieldUpdatedEvent(itemId, itemField, DateTimeOffset.Now));


        Prelude.match(
            errorOrEvent,
            @event => Persist(@event, OnEventPersisted),
            error => Sender.Tell(Prelude.Left<Error, Item>(error))
        );
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

    private void OnEventPersisted(IItemEvent @event)
    {
        _state = _state.ApplyEvent(@event);
        Sender.Tell(GetModel(@event.ItemId));
        _state.IfSome(s =>
        {
            if(LastSequenceNr % 500 == 0)
                SaveSnapshot(s);
        });
    }

    private Either<Error, ItemState> GetState(ItemId id) => _state.ToEither(ItemError.NotExist(id));

    private Either<Error, Item> GetModel(ItemId id) => GetState(id).Map(s => s.ToModel(id));
}