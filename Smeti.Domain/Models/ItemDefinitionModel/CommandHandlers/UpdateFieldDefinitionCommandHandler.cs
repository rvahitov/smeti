using Akka.Hosting;

namespace Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

internal sealed class UpdateFieldDefinitionCommandHandler
    : AbstractItemDefinitionCommandHandler<UpdateFieldDefinitionCommand>
{
    public UpdateFieldDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}