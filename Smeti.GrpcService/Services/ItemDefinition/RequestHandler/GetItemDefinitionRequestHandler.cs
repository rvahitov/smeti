using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.RequestHandler;

[UsedImplicitly]
public sealed class GetItemDefinitionRequestHandler
    : AbstractItemDefinitionRequestHandler<GetItemDefinitionRequest, GetItemDefinitionCommand>
{
    public GetItemDefinitionRequestHandler(
        IMediator mediator,
        IValidator<GetItemDefinitionRequest> validator,
        IMapper mapper
    ) : base(mediator, validator, mapper)
    {
    }
}