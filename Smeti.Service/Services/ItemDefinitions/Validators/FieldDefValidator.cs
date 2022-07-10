using FluentValidation;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.Validators;

public sealed class FieldDefValidator : AbstractValidator<FieldDef>
{
    public FieldDefValidator()
    {
        RuleFor(f => f.FieldName).NotEmpty();
    }
}