using FluentValidation;
using MediatR;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.RequestHandlers;

public sealed class AddFieldDefinitionRequestHandler
    : AbstractItemDefinitionsServiceRequestHandler<AddFieldDefinitionRequest>
{
    public AddFieldDefinitionRequestHandler(IMediator mediator, IValidator<AddFieldDefinitionRequest> validator)
        : base(mediator, validator)
    {
    }
}