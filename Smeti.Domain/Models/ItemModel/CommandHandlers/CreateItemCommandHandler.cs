using Akka.Hosting;
using JetBrains.Annotations;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

[UsedImplicitly]
internal sealed class CreateItemCommandHandler : AbstractItemCommandHandler<CreateItemCommand>
{
    public CreateItemCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}