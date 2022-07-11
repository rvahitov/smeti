using FluentValidation;
using Smeti.Service.Services.Items.Proto;
using ValueType = Smeti.Service.Services.Items.Proto.ValueType;

namespace Smeti.Service.Services.Items.Validators;

public sealed class FieldValidator : AbstractValidator<Field>
{
    public FieldValidator()
    {
        RuleFor(f => f.Name).NotEmpty();
        RuleFor(f => f.Boolean).Null().When(f => f.ValueType != ValueType.Boolean);
        RuleFor(f => f.Integer).Null().When(f => f.ValueType != ValueType.Integer);
        RuleFor(f => f.Text).Null().When(f => f.ValueType != ValueType.Text);
        RuleFor(f => f.DateTime).Null().When(f => f.ValueType != ValueType.DateTime);
        RuleFor(f => f.Reference).Null().When(f => f.ValueType != ValueType.Reference);
        RuleFor(f => f.Reference).NotEmpty().When(f => f.ValueType == ValueType.Reference && f.Reference != null);
    }
}