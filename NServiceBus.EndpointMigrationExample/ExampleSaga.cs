using NServiceBus.EndpointMigrationExample.Messages;

namespace NServiceBus.EndpointMigrationExample;

public class ExampleSaga : Saga<ExampleSagaData>,
    IAmStartedByMessages<StartExampleSaga>
{
    private readonly ILogger<ExampleSaga> _logger;

    public ExampleSaga(ILogger<ExampleSaga> logger)
    {
        _logger = logger;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ExampleSagaData> mapper)
    {
        mapper.ConfigureMapping<StartExampleSaga>(message => message.ExampleId)
            .ToSaga(sagaData => sagaData.ExampleId);
    }

    public Task Handle(StartExampleSaga message, IMessageHandlerContext context)
    {
        Data.ExampleId = message.ExampleId;
        Data.ExampleName = message.ExampleName;
        Data.Created = DateTimeOffset.Now;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("ExampleSaga started with ExampleId: {ExampleId}, ExampleName: {ExampleName}, Created: {Created}", Data.ExampleId, Data.ExampleName, Data.Created);
        }

        return Task.CompletedTask;
    }
}

public class ExampleSagaData : ContainSagaData
{
    public Guid ExampleId { get; set; }
    public string ExampleName { get; set; }
    public DateTimeOffset Created { get; set; }
}