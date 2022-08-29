using Akka.Actor;
using Akka.Hosting;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using MediatR;
using Smeti.Domain.Common;
using Smeti.Domain.Common.Errors;

namespace Smeti.Common.CommandHandler;

public abstract class AbstractCommandHandler<TActor, TCommand, TResult>
    : IRequestHandler<TCommand, Either<IDomainError, TResult>>
    where TActor : ActorBase
    where TCommand : IDomainCommand<TResult>
{
    private readonly IReadOnlyActorRegistry _actorRegistry;

    protected AbstractCommandHandler(IReadOnlyActorRegistry actorRegistry)
    {
        _actorRegistry = actorRegistry;
    }

    public Task<Either<IDomainError, TResult>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var actor = _actorRegistry.Get<TActor>();
        return actor.Ask<Either<IDomainError, TResult>>(command, AskTimeout, cancellationToken);
    }

    protected virtual TimeSpan AskTimeout => 1.Seconds();
}