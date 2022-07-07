using Akka.Actor;
using Akka.Hosting;
using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace Smeti.Domain.Models.Common;

internal abstract class AbstractCommandHandler<TActor, TCommand, TResult>
    : IRequestHandler<TCommand, Either<Error, TResult>>
    where TActor : ActorBase
    where TCommand : ICommand<TResult>
{
    private readonly IActorRef _actor;

    protected AbstractCommandHandler(IReadOnlyActorRegistry actorRegistry)
    {
        _actor = actorRegistry.Get<TActor>();
    }

    public async Task<Either<Error, TResult>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        return await
                   (from validatedCommand in ValidateCommand(command, cancellationToken)
                    from result in TryAsk(validatedCommand, cancellationToken)
                    select result);
    }

    private EitherAsync<Error, TResult> TryAsk(TCommand command, CancellationToken cancellationToken) =>
        Prelude.TryAsync(() => _actor.Ask<Either<Error, TResult>>(command, AskTimeout, cancellationToken))
               .ToEither()
               .Bind(e => e.ToAsync());

    protected virtual EitherAsync<Error, TCommand> ValidateCommand(
        TCommand command,
        CancellationToken cancellationToken
    ) => Prelude.RightAsync<Error, TCommand>(command);

    protected virtual TimeSpan AskTimeout => TimeSpan.FromSeconds(10);
}