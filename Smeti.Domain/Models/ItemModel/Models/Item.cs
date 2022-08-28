using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public sealed record Item(
    ItemId Id,
    ItemDefinitionId ItemDefinitionId,
    IReadOnlyCollection<IField> Fields,
    bool IsDeleted
);