using Autofac;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions.RequestHandlers.Infrastructure;

public sealed class RequestHandlersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<IRequestHandler<GetItemDefinitionRequest, Either<Error, ItemDefinitionOut>>>(
            ctx => new GetItemDefinitionRequestHandler(
                ctx.Resolve<IMediator>(),
                ctx.Resolve<IValidator<GetItemDefinitionRequest>>()
            )
        );
        builder.Register<IRequestHandler<CreateItemDefinitionRequest, Either<Error, ItemDefinitionOut>>>(
            ctx => new CreateItemDefinitionRequestHandler(
                ctx.Resolve<IMediator>(),
                ctx.Resolve<IValidator<CreateItemDefinitionRequest>>()
            )
        );
        builder.Register<IRequestHandler<AddFieldDefinitionRequest, Either<Error, ItemDefinitionOut>>>(
            ctx => new AddFieldDefinitionRequestHandler(
                ctx.Resolve<IMediator>(),
                ctx.Resolve<IValidator<AddFieldDefinitionRequest>>()
            )
        );
        builder.Register<IRequestHandler<UpdateFieldDefinitionRequest, Either<Error, ItemDefinitionOut>>>(
            ctx => new UpdateFieldDefinitionRequestHandler(
                ctx.Resolve<IMediator>(),
                ctx.Resolve<IValidator<UpdateFieldDefinitionRequest>>()
            )
        );
    }
}