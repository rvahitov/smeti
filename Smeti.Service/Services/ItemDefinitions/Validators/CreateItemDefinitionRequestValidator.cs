using FluentValidation;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.Validators;

public sealed class CreateItemDefinitionRequestValidator : AbstractValidator<CreateItemDefinitionRequest>
{
    public CreateItemDefinitionRequestValidator()
    {
        RuleFor(r => r.ItemDefinitionId).NotEmpty();
        RuleFor(r => r.Title).NotEmpty().When(r => r.Title is not null);
        RuleForEach(r => r.FieldDefinitions).SetValidator(new FieldDefValidator());
    }
}