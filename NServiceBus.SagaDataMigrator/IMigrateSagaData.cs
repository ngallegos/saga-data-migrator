namespace NServiceBus.SagaDataMigrator;

public interface IMigrateSagaData
{
    Task Migrate();
}