using LanguageExt;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public readonly record struct ItemDefinitionId(string Value)
{
    public override string ToString() => Value;
}

public readonly record struct ItemDefinitionTitle(string Value);

public readonly record struct ItemDefinition(
    ItemDefinitionId Id,
    Option<ItemDefinitionTitle> Title,
    Lst<IFieldDefinition> FieldDefinitions
);