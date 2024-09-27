using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace SagaDataMigrator.AzureTablesSagaDataProvider;

public class AzureTablesSagaDataProvider : IProvideLegacySagaData
{
    private readonly string _connectionString;
    private readonly ILogger<AzureTablesSagaDataProvider> _logger;
    private readonly List<AzureTablesSagaDataFilterConfiguration> _filterConfigurations;
    
    public AzureTablesSagaDataProvider(string connectionString, 
        ILoggerFactory loggerFactory,
        List<AzureTablesSagaDataFilterConfiguration> filterConfigurations)
    {
        _connectionString = connectionString;
        _filterConfigurations = filterConfigurations;
        _logger = loggerFactory.CreateLogger<AzureTablesSagaDataProvider>();
    }
    
    public Task<List<TSagaData>> GetSagaDataFromSourcePersistence<TSagaData>() where TSagaData : class, IContainSagaData, new()
    {
        var client = new TableClient(_connectionString, typeof(TSagaData).Name);
        _logger.LogInformation("Getting Azure entities from {TableName}", typeof(TSagaData).Name);
        var entities = new List<TableEntity>();
        var filterConfiguration = _filterConfigurations.FirstOrDefault(x => x.SagaDataType == typeof(TSagaData));
        if (filterConfiguration == null)
            _logger.LogInformation("No filter configuration found for {SagaDataType}, assuming no second index or custom query", typeof(TSagaData).Name);
        var customFilterQuery = filterConfiguration?.CustomFilterQuery;
        if (filterConfiguration?.FilterByNServiceBus2NdIndexKey ?? false)
        {
            var filterKey = $"Index_{typeof(TSagaData).FullName}_";
            var filterQuery = $"NServiceBus_2ndIndexKey ge '{filterKey}' and NServiceBus_2ndIndexKey lt '{filterKey}_'";
            if (!string.IsNullOrEmpty(customFilterQuery))
                filterQuery += $" and {customFilterQuery}";
            _logger.LogInformation("Filtering with query \"{FilterQuery}\"", filterQuery);
            entities.AddRange(client.Query<TableEntity>(filter: filterQuery));
        }
        else if (!string.IsNullOrEmpty(customFilterQuery))
            entities.AddRange(client.Query<TableEntity>(customFilterQuery));
        else
            entities.AddRange(client.Query<TableEntity>());

        var sagaDatas = entities.Select(x => x.ToSagaData<TSagaData>(_logger))
            .Where(x => x != null)
            .Cast<TSagaData>()
            .ToList();

        return new ValueTask<List<TSagaData>>(sagaDatas).AsTask();
    }
}