using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.RequestHandler;

[UsedImplicitly]
public sealed class CreateItemDefinitionRequestHandler
    : AbstractItemDefinitionRequestHandler<CreateItemDefinitionRequest, CreateItemDefinitionCommand>
{
    public CreateItemDefinitionRequestHandler(
        IMediator mediator,
        IValidator<CreateItemDefinitionRequest> validator,
        IMapper mapper
    ) : base(mediator, validator, mapper)
    {
    }
}