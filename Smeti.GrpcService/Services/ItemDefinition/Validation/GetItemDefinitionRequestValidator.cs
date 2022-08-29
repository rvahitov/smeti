using FluentValidation;
using JetBrains.Annotations;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition.Validation;

[UsedImplicitly]
public sealed class GetItemDefinitionRequestValidator : AbstractValidator<GetItemDefinitionRequest>
{
    public GetItemDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
    }
}