using Smeti.Domain.Common;
using Smeti.Domain.Common.Errors;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public readonly record struct ItemNotExistError(ItemId ItemId) : IDomainError;

public readonly record struct ItemAlreadyExistError(ItemId ItemId) : IDomainError;

public readonly record struct ItemDeletedError(ItemId ItemId) : IDomainError;

public readonly record struct ItemFieldDuplicateError(ItemId ItemId, IReadOnlyCollection<FieldName> FieldNames)
    : IDomainError;

public readonly record struct ItemAlreadyHasFieldError(ItemId ItemId, FieldName FieldName) : IDomainError;

public readonly record struct ItemDoesNotHaveFieldError(ItemId ItemId, FieldName FieldName) : IDomainError;

public readonly record struct ItemFieldsVerificationError(
    ItemId ItemId,
    ItemDefinitionId ItemDefinitionId,
    IReadOnlyCollection<(FieldName, InvalidFieldReason)> InvalidFields
) : IDomainError;

public sealed class ItemError
{
    public static IDomainError ItemNotExist(ItemId itemId) => new ItemNotExistError(itemId);
    public static IDomainError ItemAlreadyExist(ItemId itemId) => new ItemAlreadyExistError(itemId);

    public static IDomainError FieldDuplicates(ItemId itemId, IReadOnlyCollection<FieldName> fieldNames) =>
        new ItemFieldDuplicateError(itemId, fieldNames);

    public static IDomainError AlreadyHasField(ItemId itemId, FieldName fieldName) =>
        new ItemAlreadyHasFieldError(itemId, fieldName);

    public static IDomainError DoesNotHaveField(ItemId itemId, FieldName fieldName) =>
        new ItemDoesNotHaveFieldError(itemId, fieldName);

    public static IDomainError Deleted(ItemId itemId) => new ItemDeletedError(itemId);

    public static IDomainError InvalidFields(
        ItemId itemId,
        ItemDefinitionId itemDefinitionId,
        IReadOnlyCollection<(FieldName, InvalidFieldReason)> invalidFields) =>
        new ItemFieldsVerificationError(itemId, itemDefinitionId, invalidFields);
}