using LanguageExt;
using LanguageExt.Common;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public class ItemDefinitionErrors
{
    public static class Codes
    {
        public const int ItemDefinitionAlreadyExists = 2_000;
        public const int ItemDefinitionNotExist = 2_001;
        public const int ItemDefinitionAlreadyHasFieldDefinition = 2_002;
        public const int ItemDefinitionNotHaveFieldDefinition = 2_003;
        public const int ItemDefinitionFieldDefinitionDuplicates = 2_004;
    }

    public static Error AlreadyExists(ItemDefinitionId id) =>
        Error.New(Codes.ItemDefinitionAlreadyExists, $"Item definition '{id}' already exists");

    public static Error NotExist(ItemDefinitionId id) =>
        Error.New(Codes.ItemDefinitionNotExist, $"Item definition '{id}' does not exist");

    public static Error AlreadyHasFieldDefinition(ItemDefinitionId id, FieldName fieldName) =>
        Error.New(
            Codes.ItemDefinitionAlreadyHasFieldDefinition,
            $"Item definition '{id}' already has field definition '{fieldName}'"
        );

    public static Error NotHaveFieldDefinition(ItemDefinitionId id, FieldName fieldName) =>
        Error.New(
            Codes.ItemDefinitionNotHaveFieldDefinition,
            $"Item definition '{id}' does not have field definition '{fieldName}'"
        );

    public static Error FieldDefinitionsDuplicates(ItemDefinitionId id, IEnumerable<FieldName> fieldNames) =>
        string
           .Join(", ", fieldNames)
           .Apply(s => Error.New(
                Codes.ItemDefinitionFieldDefinitionDuplicates,
                $"Failed create item definition '{id}' with duplicate field definitions: {s}")
            );
}