using LanguageExt;
using Smeti.Domain.Common;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Domain.Models.ItemModel;

public interface IItemCommand : IDomainCommand<Item>
{
    ItemId ItemId { get; }
}

public sealed record CreateItemCommand(ItemId ItemId, ItemDefinitionId ItemDefinitionId, Lst<IField> Fields)
    : IItemCommand;

public sealed record AddFieldCommand(ItemId ItemId, IField Field) : IItemCommand;

public sealed record RemoveFieldCommand(ItemId ItemId, FieldName FieldName) : IItemCommand;

public sealed record DeleteItemCommand(ItemId ItemId) : IItemCommand;

public sealed record GetItemCommand(ItemId ItemId) : IItemCommand;