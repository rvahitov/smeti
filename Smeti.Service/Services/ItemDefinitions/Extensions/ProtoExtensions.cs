using LanguageExt;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Service.Services.ItemDefinitions.Proto;
using ValueType = Smeti.Service.Services.ItemDefinitions.Proto.ValueType;

namespace Smeti.Service.Services.ItemDefinitions.Extensions;

public static class ProtoExtensions
{
    public static IItemDefinitionCommand ToCommand(this GetItemDefinitionRequest request) =>
        new GetItemDefinitionCommand(new ItemDefinitionId(request.ItemDefinitionId));

    public static IItemDefinitionCommand ToCommand(this CreateItemDefinitionRequest request) =>
        new CreateItemDefinitionCommand(
            new ItemDefinitionId(request.ItemDefinitionId),
            Prelude.Optional(request.Title).Map(s => new ItemDefinitionTitle(s)),
            request.FieldDefinitions.Select(f => f.ToModel()).Freeze()
        );

    public static IItemDefinitionCommand ToCommand(this AddFieldDefinitionRequest request) =>
        new AddFieldDefinitionCommand(new ItemDefinitionId(request.ItemDefinitionId),
            request.FieldDefinition.ToModel()
        );

    public static IItemDefinitionCommand ToCommand(this UpdateFieldDefinitionRequest request) =>
        new UpdateFieldDefinitionCommand(
            new ItemDefinitionId(request.ItemDefinitionId),
            request.FieldDefinition.ToModel()
        );

    public static IFieldDefinition ToModel(this FieldDef fieldDef)
    {
        var fieldName = new FieldName(fieldDef.FieldName);
        return fieldDef.ValueType switch
        {
            ValueType.Boolean   => new BooleanFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.Integer   => new BooleanFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.Decimal   => new DecimalFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.DateTime  => new DateTimeFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.Date      => new DateFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.Time      => new TimeFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.TimeSpan  => new TimeSpanFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.Text      => new TextFieldDefinition(fieldName, fieldDef.IsRequired),
            ValueType.Reference => new ReferenceFieldDefinition(fieldName, fieldDef.IsRequired),
            _                   => throw new Exception("Not supported field definition")
        };
    }

    public static FieldDef ToProtoModel(this IFieldDefinition fieldDefinition) => fieldDefinition switch
    {
        BooleanFieldDefinition(var name, var isRequired) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Boolean, IsRequired = isRequired },
        IntegerFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Integer, IsRequired = isRequired },
        DecimalFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Decimal, IsRequired = isRequired },
        DateTimeFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.DateTime, IsRequired = isRequired },
        DateFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Date, IsRequired = isRequired },
        TimeFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Time, IsRequired = isRequired },
        TimeSpanFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.TimeSpan, IsRequired = isRequired },
        TextFieldDefinition(var name, var isRequired, _, _) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Text, IsRequired = isRequired },
        ReferenceFieldDefinition(var name, var isRequired) =>
            new FieldDef { FieldName = name.Value, ValueType = ValueType.Text, IsRequired = isRequired },
        _ => throw new Exception("Not supported field definition")
    };

    public static ItemDefinitionOut ToProtoModel(this ItemDefinition model)
    {
        var result = new ItemDefinitionOut
        {
            Id = model.Id.Value,
            FieldDefinitions = { model.FieldDefinitions.Select(ToProtoModel) }
        };
        model.Title.Map(title => title.Value).IfSome(title => result.Title = title);
        return result;
    }
}