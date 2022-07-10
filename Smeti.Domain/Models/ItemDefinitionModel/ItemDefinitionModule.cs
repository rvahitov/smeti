using Akka.Hosting;
using Autofac;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Domain.Models.ItemDefinitionModel.CommandHandlers;

namespace Smeti.Domain.Models.ItemDefinitionModel;

internal sealed class ItemDefinitionModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<IRequestHandler<GetItemDefinitionCommand, Either<Error, ItemDefinition>>>(
            ctx => new GetItemDefinitionCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<CreateItemDefinitionCommand, Either<Error, ItemDefinition>>>(
            ctx => new CreateItemDefinitionCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<AddFieldDefinitionCommand, Either<Error, ItemDefinition>>>(
            ctx => new AddFieldDefinitionCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<UpdateFieldDefinitionCommand, Either<Error, ItemDefinition>>>(
            ctx => new UpdateFieldDefinitionCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
        builder.Register<IRequestHandler<ValidateItemFieldsCommand, Either<Error, ItemDefinition>>>(
            ctx => new ValidateItemFieldsCommandHandler(ctx.Resolve<IReadOnlyActorRegistry>())
        );
    }
}