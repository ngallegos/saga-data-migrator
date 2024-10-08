using NServiceBus;
using SagaDataMigrator.EndpointMigrationExample;
using SagaDataMigrator;

var builder = Host.CreateApplicationBuilder(args);

var endpointName = "EndpointMigrationExample";
var endpointConfiguration = new EndpointConfiguration(endpointName);
endpointConfiguration.UseSerialization<SystemJsonSerializer>();

var runInPersistenceMigrationMode = false;

if (runInPersistenceMigrationMode)
{
    var config = new MigratorConfiguration
    {
        EndpointName = endpointName
    };

    var services = new ServiceCollection();
    services.AddScoped<IProvideLegacySagaData, LegacySagaDataProvider>();
    
    Migrator.MigrateSagas<LearningPersistence>(config, persistence =>
    {
        persistence.SagaStorageDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
            "temp", "endpoint-migration-example", "persistence"));
    }, services).GetAwaiter().GetResult();

    return;
}

var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
    "temp", "endpoint-migration-example", "transport"));

var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
persistence.SagaStorageDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
    "temp", "endpoint-migration-example", "persistence"));

builder.UseNServiceBus(endpointConfiguration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();