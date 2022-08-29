using FluentValidation;
using JetBrains.Annotations;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.Validation;

[UsedImplicitly]
public sealed class RemoveFieldDefinitionRequestValidator : AbstractValidator<RemoveFieldDefinitionRequest>
{
    public RemoveFieldDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
        RuleFor(r => r.FieldName).NotEmpty();
    }
}