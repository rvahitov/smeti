namespace Smeti.Domain.Models.ItemDefinitionModel;

public readonly struct ItemDefinitionName : IEquatable<ItemDefinitionName>
{
    public ItemDefinitionName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        Value = value;
    }

    public string Value { get; }

    public bool Equals(ItemDefinitionName other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is ItemDefinitionName other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(ItemDefinitionName left, ItemDefinitionName right) => left.Equals(right);

    public static bool operator !=(ItemDefinitionName left, ItemDefinitionName right) => !left.Equals(right);
}