using FluentValidation;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.Validators;

public sealed class AddFieldDefinitionRequestValidator : AbstractValidator<AddFieldDefinitionRequest>
{
    public AddFieldDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
        RuleFor(r => r.FieldDefinition).NotNull().SetValidator(new FieldDefValidator());
    }
}