using LanguageExt;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IFieldSpecification
{
    Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue);
    int Order { get; }
}

public sealed class ValueOfTypeSpecification<T> : IFieldSpecification
{
    public Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        Prelude.match(
            fieldValue.Map(o => o.GetType().IsAssignableTo(typeof(T))),
            isMatch => isMatch
                           ? Prelude.Success<string, Option<object>>(fieldValue)
                           : Prelude.Fail<string, Option<object>>(
                               $"Value for field '{fieldName}' must be of type {typeof(T).Name}"),
            () => Prelude.Success<string, Option<object>>(fieldValue)
        );

    public int Order => int.MinValue;
}

public sealed class ValueRequiredFieldSpecification : IFieldSpecification
{
    public Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.IsSome
            ? fieldValue
            : $"Value for  field '{fieldName}' is required";

    public int Order  => int.MinValue + 1;
}

public sealed class MinValueFieldSpecification<T> : IFieldSpecification
    where T : IComparable
{
    private readonly T _minValue;

    public MinValueFieldSpecification(T minValue)
    {
        _minValue = minValue;
    }

    public Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.Map(value => _minValue.CompareTo(value) <= 0).IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName}' violates 'Min Value' specification";

    public int Order => int.MinValue + 2;
}

public sealed class MaxValueFieldSpecification<T> : IFieldSpecification
    where T : IComparable
{
    private readonly T _maxValue;

    public MaxValueFieldSpecification(T maxValue)
    {
        _maxValue = maxValue;
    }

    public Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue.Map(value => _maxValue.CompareTo(value) >= 0).IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName}' violates 'Max Value' specification";

    public int Order => int.MinValue + 3;
}

public sealed class MinLengthFieldSpecification : IFieldSpecification
{
    private readonly int _minLength;

    public MinLengthFieldSpecification(int minLength)
    {
        _minLength = minLength;
    }

    public Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue
           .Map(value => (string) value)
           .Map(value => value.Length >= _minLength)
           .IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName} violates 'Min Length' specification";

    public int Order => int.MinValue + 4;
}

public sealed class MaxLengthFieldSpecification : IFieldSpecification
{
    private readonly int _maxLength;

    public MaxLengthFieldSpecification(int maxLength)
    {
        _maxLength = maxLength;
    }

    public Validation<string, Option<object>> ValidateFieldValue(FieldName fieldName, Option<object> fieldValue) =>
        fieldValue
           .Map(value => (string) value)
           .Map(value => value.Length <= _maxLength)
           .IfNone(true)
            ? fieldValue
            : $"Value for field '{fieldName} violates 'Max Length' specification";

    public int Order => int.MinValue + 5;
}