using NServiceBus.SagaDataMigrator;

namespace NServiceBus.EndpointMigrationExample.DataMigration;

public class ExampleSagaMigrator : SagaDataMigrator<MigrateExampleSagaData, ExampleSagaData>
{
    public ExampleSagaMigrator(IMessageSession messageSession, IPersistSagaData<ExampleSagaData> legacyPersistenceSource, ILoggerFactory loggerFactory) : base(messageSession, legacyPersistenceSource, loggerFactory)
    {
    }
}

public class MigrateExampleSagaData : MigrateSagaDataMessage<ExampleSagaData>
{
    
}