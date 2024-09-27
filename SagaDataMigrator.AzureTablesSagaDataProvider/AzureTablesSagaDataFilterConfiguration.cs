namespace SagaDataMigrator.AzureTablesSagaDataProvider;

public class AzureTablesSagaDataFilterConfiguration
{
    public required Type SagaDataType { get; set; }
    public bool FilterByNServiceBus2NdIndexKey { get; set; } = false;
    public string? CustomFilterQuery { get; set; }
}