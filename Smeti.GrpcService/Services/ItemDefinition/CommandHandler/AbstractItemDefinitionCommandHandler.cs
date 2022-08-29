using Akka.Hosting;
using Smeti.Common.CommandHandler;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Services.ItemDefinition.CommandHandler;

public abstract class AbstractItemDefinitionCommandHandler<TCommand>
    : AbstractCommandHandler<ItemDefinitionActor, TCommand, Domain.Models.ItemDefinitionModel.ItemDefinition>
    where TCommand : IItemDefinitionCommand
{
    protected AbstractItemDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}