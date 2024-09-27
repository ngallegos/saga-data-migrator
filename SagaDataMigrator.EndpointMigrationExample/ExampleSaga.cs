using NServiceBus;
using SagaDataMigrator.EndpointMigrationExample.DataMigration;
using SagaDataMigrator.EndpointMigrationExample.Messages;

namespace SagaDataMigrator.EndpointMigrationExample;

public class ExampleSaga : Saga<ExampleSagaData>,
    IAmStartedByMessages<StartExampleSaga>,
    IAmStartedByMessages<MigrateExampleSagaData>
{
    private readonly ILogger<ExampleSaga> _logger;

    public ExampleSaga(ILogger<ExampleSaga> logger)
    {
        _logger = logger;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ExampleSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.ExampleId)
            .ToMessage<StartExampleSaga>(message => message.ExampleId)
            .ToMessage<MigrateExampleSagaData>(message => message.SagaData.ExampleId);
    }

    public Task Handle(StartExampleSaga message, IMessageHandlerContext context)
    {
        Data.ExampleId = message.ExampleId;
        Data.ExampleName = message.ExampleName;
        Data.Created = DateTimeOffset.UtcNow;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("ExampleSaga started with ExampleId: {ExampleId}, ExampleName: {ExampleName}, Created: {Created}", Data.ExampleId, Data.ExampleName, Data.Created);
        }

        return Task.CompletedTask;
    }

    public Task Handle(MigrateExampleSagaData message, IMessageHandlerContext context)
    {
        Data.CopyFrom(message.SagaData);
        
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("ExampleSaga migrated saga data with ExampleId: {ExampleId}, ExampleName: {ExampleName}, Created: {Created}", Data.ExampleId, Data.ExampleName, Data.Created);
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