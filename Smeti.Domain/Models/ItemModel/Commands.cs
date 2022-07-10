using LanguageExt;
using Smeti.Domain.Models.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public interface IItemCommand : ICommand<Item>
{
    ItemId ItemId { get; }
}

public readonly record struct CreateItemCommand(
    ItemId ItemId,
    ItemDefinitionId ItemDefinitionId,
    Lst<IItemField> Fields
) : IItemCommand;

public readonly record struct GetItemCommand(ItemId ItemId) : IItemCommand;

public readonly record struct AddFieldCommand(ItemId ItemId, IItemField Field) : IItemCommand;

public readonly record struct UpdateFieldCommand(ItemId ItemId, IItemField Field) : IItemCommand;