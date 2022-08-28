using LanguageExt;
using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemModel.Utils;

public static class FieldExtensions
{
    public static Lst<FieldName> FindDuplicates(this IEnumerable<FieldName> fields) =>
        fields
           .Aggregate(
                Map.empty<FieldName, int>(), // field name with count
                (map, fieldName) => map.AddOrUpdate(fieldName, count => count + 1, 0)
            )
           .Filter(count => count > 0)
           .Keys
           .Freeze();
}