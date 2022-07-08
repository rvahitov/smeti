using FluentValidation;
using JetBrains.Annotations;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.Validators;

[UsedImplicitly]
public sealed class GetItemRequestValidator : AbstractValidator<GetItemRequest>
{
    public GetItemRequestValidator()
    {
        RuleFor(r => r.ItemId).NotEmpty();
    }
}