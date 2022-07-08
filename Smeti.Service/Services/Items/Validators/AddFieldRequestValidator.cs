using FluentValidation;
using JetBrains.Annotations;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.Validators;

[UsedImplicitly]
public sealed class AddFieldRequestValidator : AbstractValidator<AddFieldRequest>
{
    public AddFieldRequestValidator()
    {
        RuleFor(r => r.ItemId).NotEmpty();
        RuleFor(r => r.Field).NotNull().SetValidator(new FieldValidator());
    }
}