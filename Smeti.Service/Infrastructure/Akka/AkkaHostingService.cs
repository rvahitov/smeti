using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Hosting;
using Akka.Persistence.PostgreSql.Hosting;
using Akka.Remote.Hosting;
using Akka.Serialization;
using LanguageExt;
using Microsoft.Extensions.FileProviders;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;
using Smeti.Domain.Projections.ItemDefinitions;
using Smeti.Domain.Projections.Items;
using Smeti.Service.Infrastructure.Serialization;

namespace Smeti.Service.Infrastructure.Akka;

public static class AkkaHostingService
{
    public static void AddApplicationActorSystem(this IServiceCollection serviceCollection)
    {
        serviceCollection
           .AddAkka("BibleStudy", (akkaBuilder, serviceProvider) =>
            {
                var environment  = serviceProvider.GetRequiredService<IHostEnvironment>();
                var connectionString = serviceProvider
                                      .GetRequiredService<IConfiguration>()
                                      .GetConnectionString("EventStore");
                var shardOptions = new ShardOptions
                {
                    Role = "bible-study",
                    StateStoreMode = StateStoreMode.Persistence
                };

                var serializerSetup = NewtonSoftJsonSerializerSetup.Create(settings =>
                {
                    settings.Converters.Add(new DateOnlyJsonConverter());
                    settings.Converters.Add(new TimeOnlyJsonConverter());
                });
                akkaBuilder
                   .AddSetup(serializerSetup)
                   .AddHocon(LoadAdditionalConfig(environment))
                   .AddHocon(LoadMainConfig(environment))
                   .WithRemoting("localhost", 1909)
                   .WithPostgreSqlPersistence(connectionString, autoInitialize: true)
                   .WithClustering(new ClusterOptions()
                    {
                        Roles = new[] { "bible-study" },
                        SeedNodes = new[] { Address.Parse("akka.tcp://BibleStudy@localhost:1909") }
                    })
                   .WithShardRegion<ItemActor>(
                        KnownShards.Item,
                        (system, _) => id => DependencyResolver.For(system).Props<ItemActor>(id),
                        new MessageExtractor(),
                        shardOptions
                    )
                   .WithShardRegion<ItemDefinitionActor>(
                        KnownShards.ItemDefinition,
                        id => Props.Create(() => new ItemDefinitionActor(id)),
                        new MessageExtractor(),
                        shardOptions
                    )
                   .WithActors((system, registry) =>
                    {
                        var dependencyResolver = DependencyResolver.For(system);
                        var itemDefinitionDbProjection =
                            system.ActorOf(dependencyResolver.Props<ItemDefinitionDbProjectionActor>(),
                                "item-definition-db-projection"
                            );
                        registry.Register<ItemDefinitionDbProjectionActor>(itemDefinitionDbProjection);

                        var itemDbProjectionActor =
                            system.ActorOf(dependencyResolver.Props<ItemDbProjectionActor>(),
                                "item-db-projection"
                            );
                        registry.Register<ItemDbProjectionActor>(itemDbProjectionActor);
                    });
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