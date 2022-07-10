using Autofac;
using Smeti.Service.Services.ItemDefinitions.Infrastructure;
using Smeti.Service.Services.Items.Infrastructure;

namespace Smeti.Service.Services;

public sealed class ServicesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule(new ItemDefinitionsServiceModule());
        builder.RegisterModule(new ItemsServiceModule());
    }
}