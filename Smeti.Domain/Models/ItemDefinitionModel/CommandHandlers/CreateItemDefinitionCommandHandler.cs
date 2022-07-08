using Akka.Hosting;

namespace Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

internal sealed class CreateItemDefinitionCommandHandler
    : AbstractItemDefinitionCommandHandler<CreateItemDefinitionCommand>
{
    public CreateItemDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}