using NServiceBus.EndpointMigrationExample;

var builder = Host.CreateApplicationBuilder(args);

var endpointConfiguration = new EndpointConfiguration("EndpointMigrationExample");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();
var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "temp", "endpoint-migration-example", "transport"));

var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
persistence.SagaStorageDirectory(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "temp", "endpoint-migration-example", "persistence"));

builder.UseNServiceBus(endpointConfiguration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();