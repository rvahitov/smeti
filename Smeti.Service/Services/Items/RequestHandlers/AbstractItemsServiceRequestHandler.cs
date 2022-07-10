using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Domain.Extensions;
using Smeti.Domain.Models.ItemModel;
using Smeti.Service.Extensions;
using Smeti.Service.Services.Items.Extensions;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.RequestHandlers;

public abstract class AbstractItemsServiceRequestHandler<TRequest>
    : IRequestHandler<TRequest, Either<Error, ItemOut>>
    where TRequest : IRequest<Either<Error, ItemOut>>
{
    private readonly IMediator _mediator;
    private readonly IValidator<TRequest> _validator;

    protected AbstractItemsServiceRequestHandler(IMediator mediator, IValidator<TRequest> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<Either<Error, ItemOut>> Handle(TRequest request, CancellationToken cancellationToken) =>
        await _validator
             .DoValidate(request)
             .Map(ToCommand)
             .ToAsync()
             .Bind(command => _mediator.TrySend(command, cancellationToken))
             .Map(item => item.ToProtoItem());

    private static IItemCommand ToCommand(TRequest request) => request switch
    {
        GetItemRequest r     => r.ToCommand(),
        CreateItemRequest r  => r.ToCommand(),
        AddFieldRequest r    => r.ToCommand(),
        UpdateFieldRequest r => r.ToCommand(),
        _                    => throw new Exception("Not supported request")
    };
}