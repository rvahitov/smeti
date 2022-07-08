using LanguageExt;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public interface IFieldDefinition
{
    FieldName FieldName { get; }
    IEnumerable<IFieldSpecification> GetSpecifications();
}

public sealed record BooleanFieldDefinition(FieldName FieldName, bool IsRequired = false) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        if(IsRequired) yield return new ValueRequiredFieldSpecification<bool>();
    }
}

public sealed record IntegerFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<long> MinValue = default,
    Option<long> MaxValue = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<long>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinValue.Map(value => new MinValueFieldSpecification<long>(value)))
              .Concat(MaxValue.Map(value => new MaxValueFieldSpecification<long>(value)));
    }
}

public sealed record DecimalFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<decimal> MinValue = default,
    Option<decimal> MaxValue = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<decimal>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinValue.Map(value => new MinValueFieldSpecification<decimal>(value)))
              .Concat(MaxValue.Map(value => new MaxValueFieldSpecification<decimal>(value)));
    }
}

public sealed record DateTimeFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<DateTimeOffset> MinValue = default,
    Option<DateTimeOffset> MaxValue = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<DateTimeOffset>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinValue.Map(value => new MinValueFieldSpecification<DateTimeOffset>(value)))
              .Concat(MaxValue.Map(value => new MaxValueFieldSpecification<DateTimeOffset>(value)));
    }
}

public sealed record DateFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<DateOnly> MinValue = default,
    Option<DateOnly> MaxValue = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<DateOnly>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinValue.Map(value => new MinValueFieldSpecification<DateOnly>(value)))
              .Concat(MaxValue.Map(value => new MaxValueFieldSpecification<DateOnly>(value)));
    }
}

public sealed record TimeFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<TimeOnly> MinValue = default,
    Option<TimeOnly> MaxValue = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<TimeOnly>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinValue.Map(value => new MinValueFieldSpecification<TimeOnly>(value)))
              .Concat(MaxValue.Map(value => new MaxValueFieldSpecification<TimeOnly>(value)));
    }
}

public sealed record TextFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<int> MinLength = default,
    Option<int> MaxLength = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<string>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinLength.Map(value => new MinLengthFieldSpecification(value)))
              .Concat(MaxLength.Map(value => new MinLengthFieldSpecification(value)));
    }
}

public sealed record TimeSpanFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false,
    Option<TimeSpan> MinValue = default,
    Option<TimeSpan> MaxValue = default
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        var requiredSpecification = IsRequired
                                        ? Prelude.Some(
                                            (IFieldSpecification) new ValueRequiredFieldSpecification<TimeSpan>())
                                        : Prelude.None;
        return requiredSpecification
              .Concat(MinValue.Map(value => new MinValueFieldSpecification<TimeSpan>(value)))
              .Concat(MaxValue.Map(value => new MaxValueFieldSpecification<TimeSpan>(value)));
    }
}