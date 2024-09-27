using System.Text.Json;
using NServiceBus;

namespace SagaDataMigrator.EndpointMigrationExample;

public class LegacySagaDataProvider : IProvideLegacySagaData
{

    public Task<List<TSagaData>> GetSagaDataFromSourcePersistence<TSagaData>() where TSagaData : class, IContainSagaData, new()
    {
        // Reading fake data for the sake of the example, but this is where 
        // to implement custom logic for pulling your legacy saga data from
        // other persistence sources like Azure Tables, SQL, etc...
        var thisAssembly = typeof(LegacySagaDataProvider).Assembly;
        var resourceName = thisAssembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith("legacy-example-saga-data.json"));
        using var stream = thisAssembly.GetManifestResourceStream(resourceName);
        using var sr = new StreamReader(stream);
        var json = sr.ReadToEnd();
        var sagaData = JsonSerializer.Deserialize<List<TSagaData>>(json);

        return new ValueTask<List<TSagaData>>(sagaData ?? new List<TSagaData>()).AsTask();
    }
}