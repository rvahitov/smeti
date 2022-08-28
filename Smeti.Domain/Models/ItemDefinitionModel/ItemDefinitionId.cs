using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public readonly struct ItemDefinitionId : IEquatable<ItemDefinitionId>, IIdentity
{
    public ItemDefinitionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        Value = value;
    }

    public string Value { get; }

    public void Deconstruct(out string value) => value = Value;

    public bool Equals(ItemDefinitionId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is ItemDefinitionId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(ItemDefinitionId left, ItemDefinitionId right) => left.Equals(right);

    public static bool operator !=(ItemDefinitionId left, ItemDefinitionId right) => !(left == right);
}