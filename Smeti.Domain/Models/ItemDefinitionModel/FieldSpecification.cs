using LanguageExt;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IFieldSpecification
{
    Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue);
    int Order { get; }
}

public abstract class CommonFieldSpecification<T> : IFieldSpecification
{
    public virtual Validation<string, Option<object>>
        ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.Map(value => value.GetType().IsAssignableTo(typeof(T))).IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName}' must be of type {typeof(T).Name}";

    public abstract int Order { get; }
}

public sealed class ValueRequiredFieldSpecification<T> : CommonFieldSpecification<T>
{
    public override Validation<string, Option<object>> ValidateFieldValue(
        FieldName fieldName,
        Option<object> fieldValue
    ) =>
        from v1 in base.ValidateFieldValue(fieldName, fieldValue)
        from v2 in Validate(fieldName, v1)
        select v2;

    public override int Order => int.MinValue;

    private Validation<string, Option<object>> Validate(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.IsNone
            ? $"Value for  field '{fieldName}' is required"
            : fieldValue;
}

public sealed class MinValueFieldSpecification<T> : CommonFieldSpecification<T>
    where T : IComparable
{
    private readonly T _minValue;

    public MinValueFieldSpecification(T minValue)
    {
        _minValue = minValue;
    }

    public override int Order => 1;

    public override Validation<string, Option<object>> ValidateFieldValue(
        FieldName fieldName,
        Option<object> fieldValue
    ) => from v1 in base.ValidateFieldValue(fieldName, fieldValue)
         from v2 in Validate(fieldName, v1)
         select v2;

    private Validation<string, Option<object>> Validate(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.Map(value => _minValue.CompareTo(value) <= 0).IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName}' violates 'Min Value' specification";
}

public sealed class MaxValueFieldSpecification<T> : CommonFieldSpecification<T>
    where T : IComparable
{
    private readonly T _maxValue;

    public MaxValueFieldSpecification(T maxValue)
    {
        _maxValue = maxValue;
    }

    public override int Order => 2;

    public override Validation<string, Option<object>> ValidateFieldValue(
        FieldName fieldName,
        Option<object> fieldValue
    ) => from v1 in base.ValidateFieldValue(fieldName, fieldValue)
         from v2 in Validate(fieldName, v1)
         select v2;

    private Validation<string, Option<object>> Validate(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.Map(value => _maxValue.CompareTo(value) >= 0).IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName}' violates 'Max Value' specification";
}

public sealed class MinLengthFieldSpecification : CommonFieldSpecification<string>
{
    private readonly int _minLength;

    public MinLengthFieldSpecification(int minLength)
    {
        _minLength = minLength;
    }

    public override int Order => 1;

    public override Validation<string, Option<object>> ValidateFieldValue(
        FieldName fieldName,
        Option<object> fieldValue
    ) => from v1 in base.ValidateFieldValue(fieldName, fieldValue)
         from v2 in Validate(fieldName, v1)
         select v2;

    private Validation<string, Option<object>> Validate(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue
           .Map(value => (string) value)
           .Map(value => value.Length >= _minLength)
           .IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName} violates 'Min Length' specification";
}

public sealed class MaxLengthSpecification : CommonFieldSpecification<string>
{
    private readonly int _maxLength;

    public MaxLengthSpecification(int maxLength)
    {
        _maxLength = maxLength;
    }

    public override int Order => 2;

    public override Validation<string, Option<object>> ValidateFieldValue(
        FieldName fieldName,
        Option<object> fieldValue
    ) => from v1 in base.ValidateFieldValue(fieldName, fieldValue)
         from v2 in Validate(fieldName, v1)
         select v2;

    private Validation<string, Option<object>> Validate(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue
           .Map(value => (string) value)
           .Map(value => value.Length <= _maxLength)
           .IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName} violates 'Max Length' specification";
}