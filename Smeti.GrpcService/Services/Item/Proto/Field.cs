using Google.Protobuf.WellKnownTypes;
using LanguageExt;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Services.Item.Proto;

partial class Field
{
    public static Field Create(string fieldName, Option<bool> value)
    {
        var result = new Field { FieldName = fieldName, Boolean = new BooleanValue() };
        value.IfSome(v => result.Boolean.Value = v);
        return result;
    }

    public static Field Create(string fieldName, Option<long> value)
    {
        var result = new Field { FieldName = fieldName, Integer = new IntegerValue() };
        value.IfSome(v => result.Integer.Value = v);
        return result;
    }

    public static Field Create(string fieldName, Option<double> value)
    {
        var result = new Field { FieldName = fieldName, Double = new DoubleValue() };
        value.IfSome(v => result.Double.Value = v);
        return result;
    }

    public static Field Create(string fieldName, Option<decimal> value)
    {
        var result = new Field { FieldName = fieldName, Decimal = new DecimalValue() };
        value.IfSome(v => result.Decimal.Value = (double) v);
        return result;
    }

    public static Field Create(string fieldName, Option<DateTimeOffset> value)
    {
        var result = new Field { FieldName = fieldName, DateTime = new DateTimeValue() };
        value.IfSome(v => result.DateTime.Value = v.ToTimestamp());
        return result;
    }

    public static Field Create(string fieldName, Option<TimeSpan> value)
    {
        var result = new Field { FieldName = fieldName, TimeSpan = new TimeSpanValue() };
        value.IfSome(v => result.TimeSpan.Value = v.ToDuration());
        return result;
    }

    public static Field Create(string fieldName, Option<string> value)
    {
        var result = new Field { FieldName = fieldName, String = new StringValue() };
        value.IfSome(v => result.String.Value = v);
        return result;
    }

    public static Field Create(string fieldName, Option<ItemId> value)
    {
        var result = new Field { FieldName = fieldName, Reference = new ReferenceValue() };
        value.Map(id => id.Value).IfSome(v => result.Reference.Value = v);
        return result;
    }
}