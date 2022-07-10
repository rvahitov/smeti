using FluentValidation;
using MediatR;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.RequestHandlers;

public sealed class UpdateFieldDefinitionRequestHandler
    : AbstractItemDefinitionsServiceRequestHandler<UpdateFieldDefinitionRequest>
{
    public UpdateFieldDefinitionRequestHandler(IMediator mediator, IValidator<UpdateFieldDefinitionRequest> validator) :
        base(mediator, validator)
    {
    }
}