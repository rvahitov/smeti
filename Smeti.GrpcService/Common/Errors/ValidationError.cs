using FluentValidation.Results;
using Smeti.Domain.Common.Errors;

namespace Smeti.Common.Errors;

public readonly record struct ValidationError(ValidationResult ValidationResult): IDomainError;