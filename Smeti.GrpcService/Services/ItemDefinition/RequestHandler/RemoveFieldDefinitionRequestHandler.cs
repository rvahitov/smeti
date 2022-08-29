using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.RequestHandler;

[UsedImplicitly]
public sealed class RemoveFieldDefinitionRequestHandler
    : AbstractItemDefinitionRequestHandler<RemoveFieldDefinitionRequest, RemoveFieldDefinitionCommand>
{
    public RemoveFieldDefinitionRequestHandler(
        IMediator mediator,
        IValidator<RemoveFieldDefinitionRequest> validator,
        IMapper mapper
    ) : base(mediator, validator, mapper)
    {
    }
}