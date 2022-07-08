using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.RequestHandlers;

[UsedImplicitly]
public sealed class CreateItemRequestHandler : AbstractItemsServiceRequestHandler<CreateItemRequest>
{
    public CreateItemRequestHandler(IMediator mediator, IValidator<CreateItemRequest> validator)
        : base(mediator, validator)
    {
    }
}