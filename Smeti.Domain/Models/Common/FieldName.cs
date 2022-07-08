namespace Smeti.Domain.Models.Common;

public readonly record struct FieldName(string Value) : IComparable<FieldName>, IComparable
{
    public int CompareTo(FieldName other) => string.Compare(Value, other.Value, StringComparison.Ordinal);

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is FieldName other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(FieldName)}");
    }

    public override string ToString() => Value;
}