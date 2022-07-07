using Akka.Hosting;
using JetBrains.Annotations;

namespace Smeti.Domain.Models.ItemModel.CommandHandlers;

[UsedImplicitly]
internal sealed class AddFieldCommandHandler : AbstractItemCommandHandler<AddFieldCommand>
{
    public AddFieldCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}