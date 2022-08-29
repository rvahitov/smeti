using Akka.Hosting;
using JetBrains.Annotations;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Services.ItemDefinition.CommandHandler;

[UsedImplicitly]
public sealed class AddFieldDefinitionCommandHandler
    : AbstractItemDefinitionCommandHandler<AddFieldDefinitionCommand>
{
    public AddFieldDefinitionCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}