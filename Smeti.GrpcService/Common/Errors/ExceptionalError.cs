using Smeti.Domain.Common.Errors;

namespace Smeti.Common.Errors;

public readonly record struct ExceptionalError(Exception Exception) : IDomainError;