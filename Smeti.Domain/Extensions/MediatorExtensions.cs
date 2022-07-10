using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace Smeti.Domain.Extensions;

public static class MediatorExtensions
{
    public static EitherAsync<Error, T> TrySend<T>(
        this IMediator mediator,
        IRequest<Either<Error, T>> request,
        CancellationToken cancellationToken = default
    ) => Prelude
        .TryAsync(() => mediator.Send(request, cancellationToken))
        .ToEither()
        .Bind(e => e.ToAsync());
}