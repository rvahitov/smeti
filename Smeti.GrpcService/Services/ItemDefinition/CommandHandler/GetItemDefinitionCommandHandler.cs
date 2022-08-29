using Akka.Hosting;
using JetBrains.Annotations;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Services.ItemDefinition.CommandHandler;

[UsedImplicitly]
public sealed class GetItemDefinitionCommandHandler : AbstractItemDefinitionCommandHandler<GetItemDefinitionCommand>
{
    public GetItemDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}