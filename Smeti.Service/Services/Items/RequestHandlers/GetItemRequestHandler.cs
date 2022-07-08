using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.RequestHandlers;

[UsedImplicitly]
public sealed class GetItemRequestHandler : AbstractItemsServiceRequestHandler<GetItemRequest>
{
    public GetItemRequestHandler(IMediator mediator, IValidator<GetItemRequest> validator) : base(mediator, validator)
    {
    }
}