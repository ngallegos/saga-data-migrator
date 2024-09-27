# Saga Data Migrator
This is intended to help anyone using NServiceBus saga persistence migrate existing saga data from one persistence to another.

Some things to keep in mind:

* The migration should be run while the live endpoint is offline to avoid issues with in-flight messages

## Running the example
1. Run the `NServiceBus.EndpointMigrationExample` worker project. It should send one message upon startup, initializing the example saga and persisting the state to the specified folder.
2. Change the `runInPersistenceMigrationMode` variable to true at the beginning of the `Program.cs` file. 
Re-run the worker project and observe that the fake saga data in the LegacyPersistenceSource is used to start the
saga in the temporary endpoint and a new saga persistence file is created in the specified folder.

**_Note:_** This is a work in progress...