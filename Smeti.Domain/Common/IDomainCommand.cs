using LanguageExt;
using MediatR;
using Smeti.Domain.Common.Errors;

namespace Smeti.Domain.Common;

public interface IDomainCommand<TResult> : IRequest<Either<IDomainError, TResult>>
{
}