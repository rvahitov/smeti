using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemModel;

public readonly struct ItemId : IIdentity, IEquatable<ItemId>
{
    public ItemId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        Value = value;
    }

    public string Value { get; }

    public void Deconstruct(out string value)
    {
        value = Value;
    }

    public bool Equals(ItemId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is ItemId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);

    public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);

    public override string ToString() => $"ItemId({Value})";
}