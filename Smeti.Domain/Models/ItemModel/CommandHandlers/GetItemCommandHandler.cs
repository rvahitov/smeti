using Akka.Hosting;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

internal sealed class GetItemCommandHandler
    : AbstractItemCommandHandler<GetItemCommand>
{
    public GetItemCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}