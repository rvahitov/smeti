using Akka.Hosting;

namespace Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

internal sealed class AddFieldDefinitionCommandHandler
    : AbstractItemDefinitionCommandHandler<AddFieldDefinitionCommand>
{
    public AddFieldDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}