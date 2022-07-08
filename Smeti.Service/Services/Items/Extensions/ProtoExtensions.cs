using LanguageExt;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemModel;
using Smeti.Service.Services.Items.Proto;
using ValueType = Smeti.Service.Services.Items.Proto.ValueType;

namespace Smeti.Service.Services.Items.Extensions;

public static class ProtoExtensions
{
    public static GetItemCommand ToCommand(this GetItemRequest request) => new (new ItemId(request.ItemId));

    public static CreateItemCommand ToCommand(this CreateItemRequest request) =>
        Prelude
           .Optional(request.ItemId)
           .Map(id => new ItemId(id))
           .IfNone(ItemId.Next())
           .Apply(id => new CreateItemCommand(id, request.Fields.Select(f => f.ToDomainField()).Freeze()));

    public static AddFieldCommand ToCommand(this AddFieldRequest request) =>
        new (new ItemId(request.ItemId), request.Field.ToDomainField());

    public static UpdateFieldCommand ToCommand(this UpdateFieldRequest request) =>
        new(new ItemId(request.ItemId), request.Field.ToDomainField());

    public static ItemOut ToProtoItem(this Item item) => new()
    {
        Id = item.Id.Value,
        Fields = { item.Fields.Select(f => f.ToProtoField()) }
    };

    private static Field ToProtoField(this IItemField field)
    {
        switch(field)
        {
            case BooleanField(var name, var value):
                var booleanField = new Field { Name = name.Value, ValueType = ValueType.Boolean, Boolean = null };
                value.Iter(v => booleanField.Boolean = v);
                return booleanField;
            case IntegerField(var name, var value):
                var integerField = new Field { Name = name.Value, ValueType = ValueType.Integer, Integer = null };
                value.Iter(v => integerField.Integer = v);
                return integerField;
            case DecimalField(var name, var value):
                var decimalField = new Field { Name = name.Value, ValueType = ValueType.Decimal, Decimal = null };
                value.Iter(v => decimalField.Decimal = (double)v);
                return decimalField;
            case TextField(var name, var value):
                var textField = new Field { Name = name.Value, ValueType = ValueType.Text, Text = null };
                value.Iter(v => textField.Text = v);
                return textField;
            case DateTimeField(var name, var value):
                var dateTimeField = new Field { Name = name.Value, ValueType = ValueType.DateTime, DateTime = null };
                value.Iter(v => dateTimeField.DateTime = v.ToString("O"));
                return dateTimeField;
            case DateField(var name, var value):
                var dateField = new Field { Name = name.Value, ValueType = ValueType.Date, Date = null };
                value.Iter(v => dateField.Date = v.ToString("O"));
                return dateField;
            case TimeField(var name, var value):
                var timeField = new Field { Name = name.Value, ValueType = ValueType.Time, Time = null };
                value.Iter(v => timeField.Text = v.ToString("O"));
                return timeField;
            case ReferenceField(var name, var value):
                var referenceField = new Field { Name = name.Value, ValueType = ValueType.Reference, Reference = null };
                value.Iter(v => referenceField.Reference = v.Value);
                return referenceField;
            case TimeSpanField(var name, var value):
                var timeSpanField = new Field { Name = name.Value, ValueType = ValueType.TimeSpan, TimeSpan = null };
                value.Iter(v => timeSpanField.TimeSpan = v.ToString());
                return timeSpanField;
            default:
                throw new Exception("Not supported field");
        }
    }

    private static IItemField ToDomainField(this Field field) => field.ValueType switch
    {
        ValueType.Boolean => Prelude
                            .Optional(field.Boolean)
                            .Apply(value => new BooleanField(new FieldName(field.Name), value)),
        ValueType.Integer => Prelude
                            .Optional(field.Integer)
                            .Apply(value => new IntegerField(new FieldName(field.Name), value)),

        ValueType.Decimal => Prelude
                            .Optional(field.Decimal)
                            .Map(d => (decimal)d)
                            .Apply(value => new DecimalField(new FieldName(field.Name), value)),

        ValueType.Text => Prelude
                         .Optional(field.Text)
                         .Apply(value => new TextField(new FieldName(field.Name), value)),

        ValueType.DateTime => Prelude
                             .Optional(field.DateTime)
                             .Map(DateTimeOffset.Parse)
                             .Apply(value => new DateTimeField(new FieldName(field.Name), value)),

        ValueType.Date => Prelude
                         .Optional(field.Date)
                         .Map(DateOnly.Parse)
                         .Apply(value => new DateField(new FieldName(field.Name), value)),

        ValueType.Time => Prelude
                         .Optional(field.Time)
                         .Map(TimeOnly.Parse)
                         .Apply(value => new TimeField(new FieldName(field.Name), value)),

        ValueType.Reference => Prelude
                              .Optional(field.Reference)
                              .Map(id => new ItemId(id))
                              .Apply(value => new ReferenceField(new FieldName(field.Name), value)),

        ValueType.TimeSpan =>
            Prelude
               .Optional(field.TimeSpan)
               .Map(TimeSpan.Parse)
               .Apply(value => new TimeSpanField(new FieldName(field.Name), value)),

        _ => throw new Exception("Not supported field")
    };
}