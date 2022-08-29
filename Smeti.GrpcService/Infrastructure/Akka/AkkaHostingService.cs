using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.Hosting;
using Akka.Persistence.PostgreSql.Hosting;
using Akka.Remote.Hosting;
using LanguageExt;
using Microsoft.Extensions.FileProviders;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Infrastructure.Akka;

public static class AkkaHostingService
{
    public static void AddApplicationActorSystem(this IServiceCollection serviceCollection)
    {
        serviceCollection
           .AddAkka("Smeti", (akkaBuilder, serviceProvider) =>
            {
                var environment  = serviceProvider.GetRequiredService<IHostEnvironment>();
                var connectionString = serviceProvider
                                      .GetRequiredService<IConfiguration>()
                                      .GetConnectionString("EventStore");
                var shardOptions = new ShardOptions
                {
                    Role = "Smeti",
                    StateStoreMode = StateStoreMode.Persistence
                };

                akkaBuilder
                   .AddHocon(LoadAdditionalConfig(environment))
                   .AddHocon(LoadMainConfig(environment))
                   .WithRemoting("localhost", 1909)
                   .WithPostgreSqlPersistence(connectionString, autoInitialize: true)
                   .WithClustering(new ClusterOptions
                    {
                        Roles = new[] { "Smeti" },
                        SeedNodes = new[] { Address.Parse("akka.tcp://Smeti@localhost:1909") }
                    })
                   .WithShardRegion<ItemDefinitionActor>(
                        KnownShards.ItemDefinition,
                        (_, _) => id => Props.Create(() => new ItemDefinitionActor(id)),
                        new MessageExtractor(),
                        shardOptions
                    )
                   .WithShardRegion<ItemActor>(
                        KnownShards.Item,
                        (_, registry) => id => Props.Create(() => new ItemActor(id, registry)),
                        new MessageExtractor(),
                        shardOptions
                    );
            });
    }

    private static Config LoadMainConfig(IHostEnvironment environment) =>
        environment
           .ContentRootFileProvider
           .GetFileInfo("application.conf")
           .Apply(LoadConfig);

    private static Config LoadAdditionalConfig(IHostEnvironment environment) =>
        environment
           .ContentRootFileProvider
           .GetFileInfo($"application.{environment.EnvironmentName}.conf")
           .Apply(LoadConfig);

    private static Config LoadConfig(IFileInfo fileInfo) =>
        Prelude
           .Optional(fileInfo)
           .Filter(fi => fi.Exists)
           .Map(fi => File.ReadAllText(fi.PhysicalPath))
           .Map(ConfigurationFactory.ParseString)
           .IfNone(Config.Empty);
}