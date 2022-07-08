using FluentValidation;
using JetBrains.Annotations;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.Validators;

[UsedImplicitly]
public sealed class UpdateFieldRequestValidator : AbstractValidator<UpdateFieldRequest>
{
    public UpdateFieldRequestValidator()
    {
        RuleFor(r => r.ItemId).NotEmpty();
        RuleFor(r => r.Field).NotNull().SetValidator(new FieldValidator());
    }
}