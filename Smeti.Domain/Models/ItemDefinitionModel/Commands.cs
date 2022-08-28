using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IItemDefinitionCommand : IDomainCommand<ItemDefinition>
{
    ItemDefinitionId ItemDefinitionId { get; }
}

public sealed record CreateItemDefinitionCommand(
    ItemDefinitionId ItemDefinitionId,
    ItemDefinitionName ItemDefinitionName
) : IItemDefinitionCommand;

public sealed record AddFieldDefinitionCommand(ItemDefinitionId ItemDefinitionId, FieldDefinition FieldDefinition)
    : IItemDefinitionCommand;

public sealed record RemoveFieldDefinitionCommand(ItemDefinitionId ItemDefinitionId, FieldName FieldName)
    : IItemDefinitionCommand;

public sealed record GetItemDefinitionCommand(ItemDefinitionId ItemDefinitionId): IItemDefinitionCommand;