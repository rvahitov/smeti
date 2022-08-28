using LanguageExt;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public readonly record struct ItemDefinition(
    ItemDefinitionId Id,
    ItemDefinitionName Name,
    Lst<FieldDefinition> FieldDefinitions
);