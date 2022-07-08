using LanguageExt;
using LanguageExt.Common;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemModel;

public static class ItemErrors
{
    public static class Codes
    {
        public const int ItemAlreadyExists = 3_000;
        public const int ItemNotExist = 3_001;
        public const int ItemAlreadyHasField = 3_002;
        public const int ItemNotHaveField = 3_003;
        public const int ItemFieldDuplicates = 3_004;
    }

    public static Error AlreadyExists(ItemId itemId) =>
        Error.New(Codes.ItemAlreadyExists, $"Item '{itemId}' already exists");

    public static Error NotExist(ItemId itemId) =>
        Error.New(Codes.ItemNotExist, $"Item '{itemId}' does not exist");

    public static Error AlreadyHasField(ItemId itemId, FieldName fieldName) =>
        Error.New(Codes.ItemAlreadyHasField, $"Item '{itemId}' already has field '{fieldName}'");

    public static Error NotHaveField(ItemId itemId, FieldName fieldName) =>
        Error.New(Codes.ItemNotHaveField, $"Item '{itemId}' does not have field '{fieldName}'");

    public static Error FieldsDuplicates(ItemId itemId, IEnumerable<FieldName> fieldNames) =>
        string
           .Join(", ", fieldNames)
           .Apply(s => Error.New(Codes.ItemFieldDuplicates,
                $"Failed create item '{itemId}' with duplicate fields: {s}"));
}