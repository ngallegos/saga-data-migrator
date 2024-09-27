namespace NServiceBus.SagaDataMigrator;

public interface IPersistSagaData<TSagaData> where TSagaData : class, IContainSagaData
{
    Task<List<TSagaData>> GetSagaDataFromSourcePersistence();
}