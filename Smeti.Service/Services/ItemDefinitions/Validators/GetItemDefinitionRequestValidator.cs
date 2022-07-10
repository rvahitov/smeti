using FluentValidation;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.Validators;

public sealed class GetItemDefinitionRequestValidator : AbstractValidator<GetItemDefinitionRequest>
{
    public GetItemDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
    }
}