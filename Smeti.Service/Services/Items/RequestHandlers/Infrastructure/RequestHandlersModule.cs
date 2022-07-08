using Autofac;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items.RequestHandlers.Infrastructure;

public sealed class RequestHandlersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<IRequestHandler<GetItemRequest, Either<Error, ItemOut>>>(
            ctx => new GetItemRequestHandler(ctx.Resolve<IMediator>(), ctx.Resolve<IValidator<GetItemRequest>>())
        );
        builder.Register<IRequestHandler<CreateItemRequest, Either<Error, ItemOut>>>(
            ctx => new CreateItemRequestHandler(ctx.Resolve<IMediator>(), ctx.Resolve<IValidator<CreateItemRequest>>())
        );
        builder.Register<IRequestHandler<AddFieldRequest, Either<Error, ItemOut>>>(
            ctx => new AddFieldRequestHandler(ctx.Resolve<IMediator>(), ctx.Resolve<IValidator<AddFieldRequest>>())
        );
        builder.Register<IRequestHandler<UpdateFieldRequest, Either<Error, ItemOut>>>(
            ctx => new UpdateFieldRequestHandler(ctx.Resolve<IMediator>(),
                ctx.Resolve<IValidator<UpdateFieldRequest>>())
        );
    }
}