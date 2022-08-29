using FluentValidation;
using JetBrains.Annotations;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.Validation;

[UsedImplicitly]
public sealed class CreateItemDefinitionRequestValidator : AbstractValidator<CreateItemDefinitionRequest>
{
    public CreateItemDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
        RuleFor(r => r.ItemDefinitionName).NotEmpty();
    }
}