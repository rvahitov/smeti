using LanguageExt;
using Smeti.Domain.Models.ItemModel;
using Smeti.Services.Item.Proto;

namespace Smeti.Services.Item.Extensions;

public static class ProtoItemExtensions
{
    public static Option<bool> GetBooleanValue(this Field field) => Prelude.Optional(field.Boolean.Value);
    public static Option<long> GetIntegerValue(this Field field) => Prelude.Optional(field.Integer.Value);
    public static  Option<double> GetDoubleValue(this Field field) => Prelude.Optional(field.Double.Value);

    public static Option<decimal> GetDecimalValue(this Field field) =>
        Prelude.Optional(field.Decimal.Value).Map(v => (decimal)v);

    public static Option<DateTimeOffset> GetDateTimeValue(this Field field) =>
        Prelude.Optional(field.DateTime.Value).Map(v => v.ToDateTimeOffset());

    public static Option<TimeSpan> GetTimeSpanValue(this Field field) =>
        Prelude.Optional(field.TimeSpan.Value).Map(v => v.ToTimeSpan());

    public static Option<string> GetStringValue(this Field field) => Prelude.Optional(field.String.Value);

    public static Option<ItemId> GetReferenceValue(this Field field) =>
        Prelude.Optional(field.Reference.Value).Map(id => new ItemId(id));
}