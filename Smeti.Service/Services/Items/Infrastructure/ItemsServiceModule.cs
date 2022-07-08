using Autofac;
using Smeti.Service.Services.Items.RequestHandlers.Infrastructure;
using Smeti.Service.Services.Items.Validators.Infrastructure;

namespace Smeti.Service.Services.Items.Infrastructure;

public sealed class ItemsServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule(new ValidatorsModule());
        builder.RegisterModule(new RequestHandlersModule());
    }
}