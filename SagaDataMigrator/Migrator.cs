using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Persistence;

namespace SagaDataMigrator;

public class Migrator
{
    public static async Task MigrateSagas<TTargetPersistence>(MigratorConfiguration migratorConfiguration,
        Action<PersistenceExtensions<TTargetPersistence>> persistenceConfiguration,
        IServiceCollection? services = null)
        where TTargetPersistence : PersistenceDefinition
    {
        var migratorBaseType = typeof(IMigrateSagaData);
        var migratorTypes = Assembly.GetEntryAssembly()?.GetTypes()
            .Where(x => migratorBaseType.IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
            .ToList();
        await MigrateSagas<TTargetPersistence>(migratorConfiguration, persistenceConfiguration, migratorTypes, services);
    }

    public static async Task MigrateSagas<TTargetPersistence>(MigratorConfiguration migratorConfiguration,
        Action<PersistenceExtensions<TTargetPersistence>> persistenceConfiguration,
        List<Type>? migratorTypes,
        IServiceCollection? services = null)
        where TTargetPersistence : PersistenceDefinition
    {
        services ??= new ServiceCollection();
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddFilter(filter => filter >= migratorConfiguration.LogLevel);
        });

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Migrator>();

        if (!migratorTypes?.Any() ?? true)
        {
            logger.LogWarning("No Saga Migrators found");
            return;
        }

        foreach (var migratorType in migratorTypes)
            services.AddScoped(typeof(IMigrateSagaData), migratorType);
        


        var endpointConfiguration = new EndpointConfiguration(migratorConfiguration.EndpointName);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        // Don't use the same transport as live code to prevent this tool from consuming other messages
        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        transport.StorageDirectory(migratorConfiguration.LocalTransportPath());

        var persistence = endpointConfiguration.UsePersistence<TTargetPersistence>();
        persistenceConfiguration(persistence);

        var startableEndpoint = EndpointWithExternallyManagedContainer.Create(endpointConfiguration, services);
        services.AddSingleton(_ => startableEndpoint.MessageSession.Value);

        logger.LogInformation("Starting temporary endpoint");
        

        serviceProvider = services.BuildServiceProvider();
        var endpoint = await startableEndpoint.Start(services.BuildServiceProvider());

        if (!migratorConfiguration.MessageProcessingOnlyMode)
        {
            var migrators = serviceProvider.GetRequiredService<IEnumerable<IMigrateSagaData>>();
            foreach (var migrator in migrators)
            {
                logger.LogInformation("Migrating data using {Migrator}", migrator.GetType().Name);
                await migrator.Migrate();
            }
        }


        if (migratorConfiguration.SecondsToWaitForMessagesToBeProcessed > 0)
        {
            logger.LogInformation(
                "Waiting {SecondsToWaitForMessagesToBeProcessed} seconds before stopping temporary endpoint to let messages finish processing",
                migratorConfiguration.SecondsToWaitForMessagesToBeProcessed);
            await Task.Delay(TimeSpan.FromSeconds(migratorConfiguration.SecondsToWaitForMessagesToBeProcessed));
        }
        else
        {
            Console.WriteLine("Press any key to stop the endpoint - make sure all messages have been processed");
            Console.ReadKey();
        }

        logger.LogInformation("Stopping temporary endpoint");
        await endpoint.Stop();
    }
}