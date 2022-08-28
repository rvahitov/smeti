using Smeti.Domain.Common;
using Smeti.Domain.Common.Errors;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public readonly record struct ItemDefinitionNotExistsError(ItemDefinitionId ItemDefinitionId)
    : IDomainError;

public readonly record struct ItemDefinitionAlreadyExistError(ItemDefinitionId ItemDefinitionId)
    : IDomainError;

public readonly record struct ItemDefinitionAlreadyHasFieldDefinitionError(
    ItemDefinitionId ItemDefinitionId,
    FieldName FieldName
) : IDomainError;

public readonly record struct ItemDefinitionDoesNotHaveFieldDefinitionError(
    ItemDefinitionId ItemDefinitionId,
    FieldName FieldName
) : IDomainError;

public static class ItemDefinitionError
{
    public static IDomainError AlreadyExist(ItemDefinitionId itemDefinitionId) =>
        new ItemDefinitionAlreadyExistError(itemDefinitionId);

    public static IDomainError NotExist(ItemDefinitionId itemDefinitionId) =>
        new ItemDefinitionNotExistsError(itemDefinitionId);

    public static IDomainError AlreadyHasFieldDefinition(ItemDefinitionId itemDefinitionId, FieldName fieldName) =>
        new ItemDefinitionAlreadyHasFieldDefinitionError(itemDefinitionId, fieldName);

    public static IDomainError DoesNotHaveFieldDefinition(ItemDefinitionId itemDefinitionId, FieldName fieldName) =>
        new ItemDefinitionAlreadyHasFieldDefinitionError(itemDefinitionId, fieldName);
}