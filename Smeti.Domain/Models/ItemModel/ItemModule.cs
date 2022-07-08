using Akka.Hosting;
using Autofac;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Domain.Models.ItemModel.CommandHandlers;

namespace Smeti.Domain.Models.ItemModel;

internal sealed class ItemModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<IRequestHandler<GetItemCommand, Either<Error, Item>>>(
            ctx => new GetItemCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<CreateItemCommand, Either<Error, Item>>>(
            ctx => new CreateItemCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<AddFieldCommand, Either<Error, Item>>>(
            ctx => new AddFieldCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<UpdateFieldCommand, Either<Error, Item>>>(
            ctx => new UpdateFieldCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
    }
}