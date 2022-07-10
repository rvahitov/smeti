using Autofac;

namespace Smeti.Service.Services.ItemDefinitions.Validators.Infrastructure;

public sealed class ValidatorsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new GetItemDefinitionRequestValidator())
               .AsImplementedInterfaces();
        builder.Register(_ => new CreateItemDefinitionRequestValidator())
               .AsImplementedInterfaces();
        builder.Register(_ => new AddFieldDefinitionRequestValidator())
               .AsImplementedInterfaces();
        builder.Register(_ => new UpdateFieldDefinitionRequestValidator())
               .AsImplementedInterfaces();
    }
}