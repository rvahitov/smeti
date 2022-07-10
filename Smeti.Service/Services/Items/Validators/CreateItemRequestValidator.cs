using FluentValidation;
using JetBrains.Annotations;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.Validators;

[UsedImplicitly]
public sealed class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        RuleFor(r => r.ItemId).NotEmpty();
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
        RuleForEach(r => r.Fields).SetValidator(new FieldValidator());
    }
}