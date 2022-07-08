using Autofac;
using MediatR;
using Smeti.Domain;
using Smeti.Service.Services;

namespace Smeti.Service.Infrastructure;

public sealed class MainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register<ServiceFactory>(ctx =>
        {
            var container = ctx.Resolve<IComponentContext>();
            return t => container.Resolve(t);
        });
        builder.Register<IMediator>(ctx => new Mediator(ctx.Resolve<ServiceFactory>())).InstancePerLifetimeScope();
        builder.RegisterModule(new DomainModule());
        builder.RegisterModule(new ServicesModule());
    }
}