using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hocon.Extensions.Configuration;
using Serilog;
using Smeti.Service.Infrastructure;
using Smeti.Service.Infrastructure.Akka;
using Smeti.Service.Services.ItemDefinitions;
using Smeti.Service.Services.Items;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory(containerBuilder => containerBuilder.RegisterModule(new MainModule()))
);

builder.Host.ConfigureAppConfiguration((ctx, configBuilder) =>
{
    var envConf = $"application.{ctx.HostingEnvironment.EnvironmentName}.conf";
    configBuilder
       .AddHoconFile("application.conf")
       .AddHoconFile(envConf);
});

builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddApplicationActorSystem();
// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
app.MapGrpcService<ItemDefinitionsService>();
app.MapGrpcService<ItemsService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();