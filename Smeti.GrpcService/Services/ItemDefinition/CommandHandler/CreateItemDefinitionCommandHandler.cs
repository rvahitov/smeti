using Akka.Hosting;
using JetBrains.Annotations;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Services.ItemDefinition.CommandHandler;

[UsedImplicitly]
public sealed class CreateItemDefinitionCommandHandler
    : AbstractItemDefinitionCommandHandler<CreateItemDefinitionCommand>
{
    public CreateItemDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}