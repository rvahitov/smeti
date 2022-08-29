using Akka.Hosting;
using JetBrains.Annotations;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Services.ItemDefinition.CommandHandler;

[UsedImplicitly]
public sealed class RemoveFieldDefinitionCommandHandler
    : AbstractItemDefinitionCommandHandler<RemoveFieldDefinitionCommand>
{
    public RemoveFieldDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}