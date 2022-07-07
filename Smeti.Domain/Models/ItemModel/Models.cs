using LanguageExt;
using MassTransit;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemModel;

public readonly record struct ItemId(string Value)
{
    public static ItemId Next() => new (NewId.Next().ToString("D"));
    public override string ToString() => Value;
}

public interface IItemField
{
    FieldName FieldName { get; }
}

public sealed record BooleanField(FieldName FieldName, Option<bool> Value) : IItemField;

public sealed record IntegerField(FieldName FieldName, Option<long> Value) : IItemField;

public sealed record DecimalField(FieldName FieldName, Option<decimal> Value) : IItemField;

public sealed record DateTimeField(FieldName FieldName, Option<DateTimeOffset> Value) : IItemField;

public sealed record DateField(FieldName FieldName, Option<DateOnly> Value) : IItemField;

public sealed record TimeField(FieldName FieldName, Option<TimeOnly> Value) : IItemField;

public sealed record TimeSpanField(FieldName FieldName, Option<TimeSpan> Value) : IItemField;

public sealed record TextField(FieldName FieldName, Option<TimeSpan> Value) : IItemField;

public sealed record ReferenceField(FieldName FieldName, Option<ItemId> Value) : IItemField;

public sealed record Item(ItemId Id, Lst<IItemField> Fields);