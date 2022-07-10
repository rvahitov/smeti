using FluentValidation;
using MediatR;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.RequestHandlers;

public sealed class CreateItemDefinitionRequestHandler
    : AbstractItemDefinitionsServiceRequestHandler<CreateItemDefinitionRequest>
{
    public CreateItemDefinitionRequestHandler(IMediator mediator, IValidator<CreateItemDefinitionRequest> validator)
        : base(mediator, validator)
    {
    }
}