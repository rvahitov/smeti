using LanguageExt;
using Smeti.Domain.Common;
using Smeti.Domain.Common.Extensions;

namespace Smeti.Domain.Models.ItemModel;

public interface IField
{
    FieldName FieldName { get; }
    Option<object> GetValue();
}

public interface IField<T> : IField
{
    Option<T> Value { get; }
}

public abstract record AbstractField<T>(FieldName FieldName, Option<T> Value) : IField
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record BooleanField(FieldName FieldName, Option<bool> Value) : IField<bool>
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record IntegerField(FieldName FieldName, Option<long> Value) : IField<long>
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record DoubleField(FieldName FieldName, Option<double> Value) : IField<double>
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record DecimalField(FieldName FieldName, Option<decimal> Value) : IField<decimal>
{
    public DecimalField(FieldName fieldName, Option<double> value)
        : this(fieldName, value.Map(v => (decimal)v))
    {
    }

    public Option<object> GetValue() => Value.Box();
}

public sealed record DateTimeField(FieldName FieldName, Option<DateTimeOffset> Value) : IField<DateTimeOffset>
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record TimeSpanField(FieldName FieldName, Option<TimeSpan> Value) : IField<TimeSpan>
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record StringField(FieldName FieldName, Option<string> Value) : IField<string>
{
    public Option<object> GetValue() => Value.Box();
}

public sealed record ReferenceField(FieldName FieldName, Option<ItemId> Value) : IField<ItemId>
{
    public Option<object> GetValue() => Value.Box();
}