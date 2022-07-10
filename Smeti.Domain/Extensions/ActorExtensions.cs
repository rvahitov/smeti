using Akka.Actor;
using LanguageExt;
using LanguageExt.Common;
using Smeti.Domain.Models.Common;

namespace Smeti.Domain.Extensions;

internal static class ActorExtensions
{
    public static EitherAsync<Error, TResponse> TryAsk<TResponse>(
        this IActorRef actorRef,
        ICommand<TResponse> command,
        Option<TimeSpan> askTimeout = default,
        CancellationToken cancellationToken = default
    ) => Prelude
        .TryAsync(() => actorRef.Ask<Either<Error, TResponse>>(
             command,
             askTimeout.IfNone(TimeSpan.FromSeconds(20)),
             cancellationToken)
         )
        .ToEither()
        .Bind(e => e.ToAsync());
}