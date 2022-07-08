using Akka.Hosting;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

internal sealed class AddFieldCommandHandler : AbstractItemCommandHandler<AddFieldCommand>
{
    public AddFieldCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}