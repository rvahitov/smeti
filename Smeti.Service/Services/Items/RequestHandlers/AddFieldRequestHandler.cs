using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.RequestHandlers;

[UsedImplicitly]
public sealed class AddFieldRequestHandler : AbstractItemsServiceRequestHandler<AddFieldRequest>
{
    public AddFieldRequestHandler(IMediator mediator, IValidator<AddFieldRequest> validator) : base(mediator, validator)
    {
    }
}