using FluentValidation;
using JetBrains.Annotations;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.Validation;

[UsedImplicitly]
public sealed class AddFieldDefinitionRequestValidator : AbstractValidator<AddFieldDefinitionRequest>
{
    public AddFieldDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
        RuleFor(r => r.FieldDefinition).SetValidator(new FieldDefinitionValidator());
    }
}