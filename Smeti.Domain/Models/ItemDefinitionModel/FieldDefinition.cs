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
        yield return new ValueOfTypeSpecification<bool>();
        if(IsRequired) yield return new ValueRequiredFieldSpecification();
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
        yield return new ValueOfTypeSpecification<long>();
        if(IsRequired) yield return new ValueRequiredFieldSpecification();

        foreach(var spec in MinValue.Map(value => new MinValueFieldSpecification<long>(value)))
            yield return spec;

        foreach(var spec in MaxValue.Map(value => new MaxValueFieldSpecification<long>(value)))
            yield return spec;
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
        yield return new ValueOfTypeSpecification<decimal>();
        if(IsRequired) yield return new ValueRequiredFieldSpecification();

        foreach(var spec in MinValue.Map(value => new MinValueFieldSpecification<decimal>(value)))
            yield return spec;

        foreach(var spec in MaxValue.Map(value => new MaxValueFieldSpecification<decimal>(value)))
            yield return spec;
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
        yield return new ValueOfTypeSpecification<DateTimeOffset>();
        if(IsRequired) yield return new ValueRequiredFieldSpecification();

        foreach(var spec in MinValue.Map(value => new MinValueFieldSpecification<DateTimeOffset>(value)))
            yield return spec;

        foreach(var spec in MaxValue.Map(value => new MaxValueFieldSpecification<DateTimeOffset>(value)))
            yield return spec;
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
        yield return new ValueOfTypeSpecification<string>();
        if(IsRequired)
            yield return new ValueRequiredFieldSpecification();

        foreach(var spec in MinLength.Map(value => new MinLengthFieldSpecification(value)))
            yield return spec;
        foreach(var spec in MaxLength.Map(value => new MaxLengthFieldSpecification(value)))
            yield return spec;
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
        yield return new ValueOfTypeSpecification<TimeSpan>();
        if(IsRequired) yield return new ValueRequiredFieldSpecification();

        foreach(var spec in MinValue.Map(value => new MinValueFieldSpecification<TimeSpan>(value)))
            yield return spec;

        foreach(var spec in MaxValue.Map(value => new MaxValueFieldSpecification<TimeSpan>(value)))
            yield return spec;
    }
}

public sealed record ReferenceFieldDefinition(
    FieldName FieldName,
    bool IsRequired = false
) : IFieldDefinition
{
    public IEnumerable<IFieldSpecification> GetSpecifications()
    {
        yield return new ValueOfTypeSpecification<string>();
        if(IsRequired) yield return new ValueRequiredFieldSpecification();
    }
}