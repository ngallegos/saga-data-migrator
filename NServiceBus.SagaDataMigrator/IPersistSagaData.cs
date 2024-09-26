namespace NServiceBus.SagaDataMigrator;

public interface IPersistSagaData
{
    Task<List<TSagaData>> GetSagaDataFromSourcePersistence<TSagaData>() where TSagaData : class, IContainSagaData, new();
}