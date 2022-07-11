using LanguageExt;
using MassTransit;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public readonly record struct ItemId(string Value)
{
    public static ItemId Next() => new (NewId.Next().ToString("D"));
    public override string ToString() => Value;
}

public interface IItemField
{
    FieldName FieldName { get; }
    Option<object> GetValue();
}

public sealed record BooleanField(FieldName FieldName, Option<bool> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record IntegerField(FieldName FieldName, Option<long> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record DecimalField(FieldName FieldName, Option<decimal> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record DateTimeField(FieldName FieldName, Option<DateTimeOffset> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record TimeSpanField(FieldName FieldName, Option<TimeSpan> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record TextField(FieldName FieldName, Option<string> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record ReferenceField(FieldName FieldName, Option<ItemId> Value) : IItemField
{
    public Option<object> GetValue() => Value.Map(v => (object)v);
}

public sealed record Item(ItemId Id, ItemDefinitionId ItemDefinitionId, Lst<IItemField> Fields);