using FluentValidation;
using Hocon.Extensions.Configuration;
using MediatR;
using Serilog;
using Smeti.Infrastructure.Akka;
using Smeti.Services.ItemDefinition;
using AutoMapperConfigurationProvider = AutoMapper.IConfigurationProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((context,  cfgBuilder) =>
{
    cfgBuilder.AddHoconFile("application.conf", true);
    cfgBuilder.AddHoconFile($"application.{context.HostingEnvironment.EnvironmentName}.conf", true);
});

builder.Host.UseSerilog((context,  loggerCfg) => loggerCfg.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplicationActorSystem();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddGrpc();

var app = builder.Build();
// check if our mappings are valid
app.Services.GetRequiredService<AutoMapperConfigurationProvider>().AssertConfigurationIsValid();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
app.MapGrpcService<ItemDefinitionService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();