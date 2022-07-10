using Autofac;
using Smeti.Service.Services.ItemDefinitions.RequestHandlers.Infrastructure;
using Smeti.Service.Services.ItemDefinitions.Validators.Infrastructure;

namespace Smeti.Service.Services.ItemDefinitions.Infrastructure;

public sealed class ItemDefinitionsServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule(new ValidatorsModule());
        builder.RegisterModule(new RequestHandlersModule());
    }
}