using Autofac;

namespace Smeti.Service.Services.Items.Validators.Infrastructure;

public sealed class ValidatorsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new GetItemRequestValidator()).AsImplementedInterfaces();
        builder.Register(_ => new CreateItemRequestValidator()).AsImplementedInterfaces();
        builder.Register(_ => new AddFieldRequestValidator()).AsImplementedInterfaces();
        builder.Register(_ => new UpdateFieldRequestValidator()).AsImplementedInterfaces();
    }
}