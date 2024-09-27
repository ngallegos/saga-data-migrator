using NServiceBus;

namespace SagaDataMigrator.EndpointMigrationExample.Messages;

public class StartExampleSaga : IMessage
{
    public Guid ExampleId { get; set; } = Guid.NewGuid();
    public string ExampleName { get; set; } = DateTimeOffset.Now.ToString("u") + " - ExampleSaga";
}