using FluentValidation;
using MediatR;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.RequestHandlers;

public sealed class GetItemDefinitionRequestHandler
    : AbstractItemDefinitionsServiceRequestHandler<GetItemDefinitionRequest>
{
    public GetItemDefinitionRequestHandler(IMediator mediator, IValidator<GetItemDefinitionRequest> validator)
        : base(mediator, validator)
    {
    }
}