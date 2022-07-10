using LanguageExt;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IItemDefinitionCommand : ICommand<ItemDefinition>
{
    ItemDefinitionId ItemDefinitionId { get; }
}

public readonly record struct CreateItemDefinitionCommand(
    ItemDefinitionId ItemDefinitionId,
    Option<ItemDefinitionTitle> ItemDefinitionTitle,
    Lst<IFieldDefinition> FieldDefinitions
) : IItemDefinitionCommand;

public readonly record struct AddFieldDefinitionCommand(
    ItemDefinitionId ItemDefinitionId,
    IFieldDefinition FieldDefinition
) : IItemDefinitionCommand;

public readonly record struct UpdateFieldDefinitionCommand(
    ItemDefinitionId ItemDefinitionId,
    IFieldDefinition FieldDefinition
) : IItemDefinitionCommand;

public readonly record struct GetItemDefinitionCommand(
    ItemDefinitionId ItemDefinitionId
) : IItemDefinitionCommand;

internal readonly record struct ValidateItemFieldsCommand(
    ItemDefinitionId ItemDefinitionId,
    Lst<IItemField> Fields
) : IItemDefinitionCommand;