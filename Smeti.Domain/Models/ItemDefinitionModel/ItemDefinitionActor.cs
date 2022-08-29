using Akka.Actor;
using Akka.Persistence;
using JetBrains.Annotations;
using LanguageExt;
using Smeti.Domain.Common.Errors;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public sealed class ItemDefinitionActor : ReceivePersistentActor
{
    private Option<ItemDefinitionActorState> _state;

    [UsedImplicitly]
    public ItemDefinitionActor(string persistenceId)
    {
        PersistenceId = persistenceId;
        _state = Prelude.None;

        Recover<IItemDefinitionEvent>(ApplyEvent);
        Recover<SnapshotOffer>(ApplySnapshot);
        Command<IItemDefinitionCommand>(HandleCommand);
    }

    public override string PersistenceId { get; }

    private void HandleCommand(IItemDefinitionCommand command)
    {
        if(command is GetItemDefinitionCommand(var id))
        {
            Reply(id);
            return;
        }

        var errorOrEvent = GetEventForCommand(command);
        switch(errorOrEvent.Case)
        {
            case IDomainError error:
                Reply(error);
                return;
            case IItemDefinitionEvent @event:
                Persist(@event, OnEventPersisted);
                break;
        }
    }

    private Either<IDomainError, IItemDefinitionEvent> GetEventForCommand(IItemDefinitionCommand command) =>
        command switch
        {
            CreateItemDefinitionCommand c  => GetEventForCommand(c),
            AddFieldDefinitionCommand c    => GetEventForCommand(c),
            RemoveFieldDefinitionCommand c => GetEventForCommand(c),
            _ => Prelude.Left<IDomainError, IItemDefinitionEvent>(
                CommonError.UnknownCommand(command.GetType().FullName!)
            )
        };

    private Either<IDomainError, IItemDefinitionEvent> GetEventForCommand(CreateItemDefinitionCommand command)
    {
        var (id, name) = command;
        return _state.Case switch
        {
            null => new ItemDefinitionCreatedEvent(id, name, DateTimeOffset.Now),
            _    => Prelude.Left<IDomainError, IItemDefinitionEvent>(ItemDefinitionError.AlreadyExist(id))
        };
    }

    private Either<IDomainError, IItemDefinitionEvent> GetEventForCommand(AddFieldDefinitionCommand command)
    {
        var (id, fd) = command;
        var (fieldName, _) = fd;
        return _state.Case switch
        {
            null => Prelude.Left<IDomainError, IItemDefinitionEvent>(ItemDefinitionError.NotExist(id)),
            ItemDefinitionActorState state when state.ContainsField(fieldName) =>
                Prelude.Left<IDomainError, IItemDefinitionEvent>(
                    ItemDefinitionError.AlreadyHasFieldDefinition(id, fieldName)
                ),
            _ => new FieldDefinitionAddedEvent(id, fd, DateTimeOffset.Now)
        };
    }

    private Either<IDomainError, IItemDefinitionEvent> GetEventForCommand(RemoveFieldDefinitionCommand command)
    {
        var (id, fieldName) = command;
        return _state.Case switch
        {
            null => Prelude.Left<IDomainError, IItemDefinitionEvent>(ItemDefinitionError.NotExist(id)),
            ItemDefinitionActorState state when state.ContainsField(fieldName) == false =>
                Prelude.Left<IDomainError, IItemDefinitionEvent>(ItemDefinitionError.DoesNotHaveFieldDefinition(
                    id, fieldName
                )),
            _ => new FieldDefinitionRemovedEvent(id, fieldName, DateTimeOffset.Now)
        };
    }

    private void OnEventPersisted(IItemDefinitionEvent @event)
    {
        ApplyEvent(@event);
        SaveSnapshot();
        Reply(@event.ItemDefinitionId);
    }

    private void ApplyEvent(IItemDefinitionEvent @event)
    {
        _state = _state.ApplyEvent(@event);
    }

    private void ApplySnapshot(SnapshotOffer offer)
    {
        if(offer.Snapshot is ItemDefinitionActorState state)
        {
            _state = state;
        }
    }

    private void SaveSnapshot()
    {
        if(LastSequenceNr % 500 == 0)
        {
            _state.Iter(SaveSnapshot);
        }
    }

    private void Reply(ItemDefinitionId itemDefinitionId)
    {
        Sender.Tell(
            _state
               .Map(s => s.ToItemDefinition(itemDefinitionId))
               .ToEither(ItemDefinitionError.NotExist(itemDefinitionId))
        );
    }

    private void Reply(IDomainError domainError)
    {
        Sender.Tell(Prelude.Left<IDomainError, ItemDefinition>(domainError));
    }
}