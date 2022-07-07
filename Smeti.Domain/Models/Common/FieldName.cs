namespace Smeti.Domain.Models.Common;

public readonly record struct FieldName(string Value)
{
    public override string ToString() => Value;
}