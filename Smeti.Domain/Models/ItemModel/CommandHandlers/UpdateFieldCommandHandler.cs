using Akka.Hosting;
using JetBrains.Annotations;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

[UsedImplicitly]
internal sealed class UpdateFieldCommandHandler : AbstractItemCommandHandler<UpdateFieldCommand>
{
    public UpdateFieldCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}