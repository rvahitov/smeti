using Akka.Actor;
using Autofac;
using Smeti.Domain.Projections.Services;
using Smeti.Projection;
using Smeti.Service.Services.ItemDefinitions.Infrastructure;
using Smeti.Service.Services.Items.Infrastructure;
using Smeti.Service.Services.Projections;

namespace Smeti.Service.Services;

public sealed class ServicesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule(new ItemDefinitionsServiceModule());
        builder.RegisterModule(new ItemsServiceModule());
        builder.Register<IPostgresConnectionFactory>(ctx =>
            new PostgresConnectionFactory(ctx.Resolve<IConfiguration>())
        );
        builder.Register<IEventQueryProvider>(ctx =>
            new EventQueryProvider(ctx.Resolve<ActorSystem>())
        );
    }
}