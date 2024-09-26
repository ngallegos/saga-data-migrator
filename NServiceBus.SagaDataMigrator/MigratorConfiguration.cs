using Microsoft.Extensions.Logging;

namespace NServiceBus.SagaDataMigrator;

public class MigratorConfiguration
{
    public required string EndpointName { get; set; }
    public int SecondsToWaitForMessagesToBeProcessed { get; set; } = 0;
    public bool MessageProcessingOnlyMode { get; set; } = false;
    public string LocalTransportRoot { get; set; } = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "temp", "nsb-saga-migration");

    public string LocalTransportPath(string? folder = null)
    {
        folder ??= EndpointName;
        return Path.Combine(LocalTransportRoot, folder);
    }
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}