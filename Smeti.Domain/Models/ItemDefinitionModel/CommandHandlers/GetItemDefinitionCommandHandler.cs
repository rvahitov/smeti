using Akka.Hosting;

namespace Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

internal sealed class GetItemDefinitionCommandHandler : AbstractItemDefinitionCommandHandler<GetItemDefinitionCommand>
{
    public GetItemDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}