namespace NServiceBus.SagaDataMigrator;

public abstract class MigrateSagaDataMessage<TSagaData> : IMessage where TSagaData : IContainSagaData
{
    public TSagaData SagaData { get; set; }
}