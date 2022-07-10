using Akka.Hosting;

namespace Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

internal sealed class ValidateItemFieldsCommandHandler
    : AbstractItemDefinitionCommandHandler<ValidateItemFieldsCommand>
{
    public ValidateItemFieldsCommandHandler(IReadOnlyActorRegistry actorRegistry) : base(actorRegistry)
    {
    }
}