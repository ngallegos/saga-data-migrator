using Microsoft.Extensions.Logging;

namespace NServiceBus.SagaDataMigrator;

public interface IMigrateSagaData
{
    Task Migrate();
}

public abstract class SagaDataMigrator<TSagaStarter, TSagaData> : IMigrateSagaData 
    where TSagaStarter : MigrateSagaDataMessage<TSagaData>
    where TSagaData : class, IContainSagaData, new()
{
    private readonly IMessageSession _messageSession;
    private readonly IPersistSagaData _sourcePersistence;
    private readonly ILogger<SagaDataMigrator<TSagaStarter, TSagaData>> _logger;
    protected SagaDataMigrator(IMessageSession messageSession, 
        IPersistSagaData sourcePersistence,
        ILoggerFactory loggerFactory)
    {
        _messageSession = messageSession;
        _sourcePersistence = sourcePersistence;
        _logger = loggerFactory.CreateLogger<SagaDataMigrator<TSagaStarter,TSagaData>>();
    }
    
    public async Task Migrate()
    {
        var sagaDatas = await _sourcePersistence.GetSagaDataFromSourcePersistence<TSagaData>();
        var counter = 0;
        foreach (var data in sagaDatas)
        {
            var starter = Activator.CreateInstance<TSagaStarter>();
            starter.SagaData = data;
            var sendOptions = new SendOptions();
            sendOptions.RouteToThisEndpoint();
            sendOptions.RequireImmediateDispatch();
            await _messageSession.Send(starter, sendOptions);
            counter++;
            _logger.LogInformation("Sent {SagaStarterType} with {SagaDataType} for {SagaDataID}", typeof(TSagaStarter).Name, typeof(TSagaData).Name, data.Id);
        }
        _logger.LogInformation("Sent {EntityCount} {SagaStarterType} messages with {SagaDataType} data", counter, typeof(TSagaStarter).Name, typeof(TSagaData).Name);
    }
}
