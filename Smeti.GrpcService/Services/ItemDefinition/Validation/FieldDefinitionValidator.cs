using FluentValidation;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.Validation;

public sealed class FieldDefinitionValidator : AbstractValidator<FieldDefinition>
{
    public FieldDefinitionValidator()
    {
        RuleFor(fd => fd.FieldName).NotEmpty();
    }
}