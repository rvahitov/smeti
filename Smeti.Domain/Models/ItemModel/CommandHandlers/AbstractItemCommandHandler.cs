using Akka.Hosting;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

internal abstract class AbstractItemCommandHandler<TCommand>
    : AbstractCommandHandler<ItemActor, TCommand, Item>
    where TCommand : IItemCommand
{
    protected AbstractItemCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}