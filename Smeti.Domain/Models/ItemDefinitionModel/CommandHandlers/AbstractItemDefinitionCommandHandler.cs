using Akka.Hosting;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

internal abstract class AbstractItemDefinitionCommandHandler<TCommand>
    : AbstractCommandHandler<ItemDefinitionActor, TCommand, ItemDefinition>
    where TCommand : IItemDefinitionCommand
{
    protected AbstractItemDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}