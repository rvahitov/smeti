using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace Smeti.Domain.Models.Common;

public interface ICommand<T> : IRequest<Either<Error, T>>
{
}