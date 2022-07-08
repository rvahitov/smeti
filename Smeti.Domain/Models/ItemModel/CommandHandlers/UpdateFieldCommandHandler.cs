using Akka.Hosting;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

internal sealed class UpdateFieldCommandHandler : AbstractItemCommandHandler<UpdateFieldCommand>
{
    public UpdateFieldCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}