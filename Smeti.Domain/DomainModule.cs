using Autofac;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Domain;

public sealed class DomainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule(new ItemDefinitionModule());
        builder.RegisterModule(new ItemModule());
    }
}