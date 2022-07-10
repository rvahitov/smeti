using Akka.Actor;
using Akka.Hosting;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Domain.Extensions;

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
                    from result in _actor.TryAsk(validatedCommand, AskTimeout, cancellationToken)
                    select result);
    }

    protected virtual EitherAsync<Error, TCommand> ValidateCommand(
        TCommand command,
        CancellationToken cancellationToken
    ) => Prelude.RightAsync<Error, TCommand>(command);

    protected virtual TimeSpan AskTimeout => TimeSpan.FromSeconds(10);
}