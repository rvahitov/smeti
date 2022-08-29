using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.RequestHandler;

[UsedImplicitly]
public sealed class AddFieldDefinitionRequestHandler
    : AbstractItemDefinitionRequestHandler<AddFieldDefinitionRequest, AddFieldDefinitionCommand>
{
    public AddFieldDefinitionRequestHandler(
        IMediator mediator,
        IValidator<AddFieldDefinitionRequest> validator,
        IMapper mapper
    ) : base(mediator, validator, mapper)
    {
    }
}