using NServiceBus;

namespace SagaDataMigrator;

public interface IProvideLegacySagaData
{
    Task<List<TSagaData>> GetSagaDataFromSourcePersistence<TSagaData>()  where TSagaData : class, IContainSagaData, new();
}