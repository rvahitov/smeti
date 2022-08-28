namespace Smeti.Domain.Common;

public readonly struct FieldName : IEquatable<FieldName>
{
    private static readonly StringComparer ValueComparer = StringComparer.OrdinalIgnoreCase;

    public FieldName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        Value = value;
    }

    public string Value { get; }

    public bool Equals(FieldName other) => ValueComparer.Equals(Value, other.Value);

    public override int GetHashCode() => ValueComparer.GetHashCode(Value);

    public override bool Equals(object? obj) => obj is FieldName other && Equals(other);

    public static bool operator ==(FieldName left, FieldName right) => left.Equals(right);

    public static bool operator !=(FieldName left, FieldName right) => !left.Equals(right);
}