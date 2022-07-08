using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.RequestHandlers;

[UsedImplicitly]
public sealed class UpdateFieldRequestHandler : AbstractItemsServiceRequestHandler<UpdateFieldRequest>
{
    public UpdateFieldRequestHandler(IMediator mediator, IValidator<UpdateFieldRequest> validator) : base(mediator,
        validator)
    {
    }
}