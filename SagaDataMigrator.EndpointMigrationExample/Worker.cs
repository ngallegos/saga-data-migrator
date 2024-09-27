using NServiceBus;
using SagaDataMigrator.EndpointMigrationExample.Messages;

namespace SagaDataMigrator.EndpointMigrationExample;

public class Worker : BackgroundService
{
    private readonly IMessageSession _messageSession;

    public Worker(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageSession.SendLocal(new StartExampleSaga(), cancellationToken: cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}