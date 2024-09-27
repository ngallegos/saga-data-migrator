using NServiceBus;

namespace SagaDataMigrator.EndpointMigrationExample.DataMigration;

public class ExampleSagaMigrator : SagaDataMigrator<MigrateExampleSagaData, ExampleSagaData>
{
    public ExampleSagaMigrator(IMessageSession messageSession, IProvideLegacySagaData legacyPersistenceSource, ILoggerFactory loggerFactory) : base(messageSession, legacyPersistenceSource, loggerFactory)
    {
    }
}

public class MigrateExampleSagaData : MigrateSagaDataMessage<ExampleSagaData>
{
    
}