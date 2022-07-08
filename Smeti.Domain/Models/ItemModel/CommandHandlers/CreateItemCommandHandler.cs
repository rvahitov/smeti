using Akka.Hosting;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

internal sealed class CreateItemCommandHandler : AbstractItemCommandHandler<CreateItemCommand>
{
    public CreateItemCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}