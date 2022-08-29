using AutoMapper;
using FluentValidation;
using Grpc.Core;
using LanguageExt;
using MediatR;
using Smeti.Common.Extensions;
using Smeti.Domain.Models.ItemDefinitionModel;

namespace Smeti.Services.ItemDefinition.RequestHandler;

public abstract class AbstractItemDefinitionRequestHandler<TRequest, TCommand>
    : IRequestHandler<TRequest, Either<RpcException, Proto.ItemDefinition>>
    where TRequest : IRequest<Either<RpcException, Proto.ItemDefinition>>
    where TCommand : IItemDefinitionCommand
{
    private readonly IMediator _mediator;
    private readonly IValidator<TRequest> _validator;
    private readonly IMapper _mapper;

    protected AbstractItemDefinitionRequestHandler(
        IMediator mediator,
        IValidator<TRequest> validator,
        IMapper mapper
    )
    {
        _mediator = mediator;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Either<RpcException, Proto.ItemDefinition>> Handle(
        TRequest request, CancellationToken cancellationToken)
    {
        var steps =
            from validRequest in _validator.TryValidateAsync(request, cancellationToken)
            from command in _mapper.TryMapAsync<TCommand>(validRequest)
            from itemDefinition in _mediator.TrySendAsync(command, cancellationToken).Bind(e => e.ToAsync())
            from protoItemDefinition in _mapper.TryMapAsync<Proto.ItemDefinition>(itemDefinition)
            select protoItemDefinition;
        return await steps.MapLeft(error => _mapper.Map<RpcException>(error));
    }
}