using FluentValidation;
using Grpc.Core;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Domain.Extensions;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Service.Extensions;
using Smeti.Service.Services.ItemDefinitions.Extensions;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.RequestHandlers;

public class AbstractItemDefinitionsServiceRequestHandler<TRequest>
    : IRequestHandler<TRequest, Either<Error, ItemDefinitionOut>>
    where TRequest : IRequest<Either<Error, ItemDefinitionOut>>
{
    private readonly IMediator _mediator;
    private readonly IValidator<TRequest> _validator;

    public AbstractItemDefinitionsServiceRequestHandler(
        IMediator mediator,
        IValidator<TRequest> validator
    )
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<Either<Error, ItemDefinitionOut>> Handle(
        TRequest request,
        CancellationToken cancellationToken
    ) =>
        await
            _validator
               .DoValidate(request)
               .Bind(ToCommand)
               .ToAsync()
               .Bind(command => _mediator.TrySend(command, cancellationToken))
               .Map(item => item.ToProtoModel());

    private static Either<Error, IItemDefinitionCommand> ToCommand(TRequest request)
        => request switch
        {
            GetItemDefinitionRequest r     => Prelude.Right<Error, IItemDefinitionCommand>(r.ToCommand()),
            CreateItemDefinitionRequest r  => Prelude.Right<Error, IItemDefinitionCommand>(r.ToCommand()),
            AddFieldDefinitionRequest r    => Prelude.Right<Error, IItemDefinitionCommand>(r.ToCommand()),
            UpdateFieldDefinitionRequest r => Prelude.Right<Error, IItemDefinitionCommand>(r.ToCommand()),
            _ => Error.New(
                (int)StatusCode.Unimplemented,
                "Not supported request"
            )
        };
}