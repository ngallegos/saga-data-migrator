using NServiceBus.SagaDataMigrator;

namespace NServiceBus.EndpointMigrationExample;

public class LegacyPersistenceSource : IPersistSagaData<ExampleSagaData>
{
    public Task<List<ExampleSagaData>> GetSagaDataFromSourcePersistence()
    {
        // Creating fake data for the sake of the example, but this is where 
        // to implement custom logic for pulling your legacy saga data
        
        var sagaData = new List<ExampleSagaData>
        {
            new ExampleSagaData
            {
                Id = Guid.NewGuid(),
                Originator = "Somewhere",
                OriginalMessageId = Guid.NewGuid().ToString(),
                ExampleId = Guid.NewGuid(),
                ExampleName = DateTimeOffset.Now.ToString("u") + " - ExampleMigratedSaga",
                Created = DateTimeOffset.UtcNow
            }
        };

        return new ValueTask<List<ExampleSagaData>>(sagaData).AsTask();
    }
}