using System.Reflection;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace SagaDataMigrator.AzureTablesSagaDataProvider;

public static class Extensions
{
    public static T? ToSagaData<T>(this TableEntity entity, ILogger? logger = null) where T : IContainSagaData
    {
        var obj = Activator.CreateInstance<T>();
        if (!Guid.TryParse(entity.PartitionKey, out var id))
            return default;
        obj.Id = id;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanWrite)
            .ToList();
        foreach (var propertyInfo in properties)
        {
            var propertyName = propertyInfo.Name;
            if (entity.ContainsKey(propertyName) && entity[propertyName] != null)
            {
                var tableValue = entity[propertyName];
                var tableType = tableValue.GetType();
                if (propertyInfo.PropertyType.IsAssignableFrom(tableType))
                    propertyInfo.SetValue(obj, tableValue);
                else if (tableValue is string strVal)
                {
                    try
                    {
                        propertyInfo.SetValue(obj, JsonSerializer.Deserialize(strVal, propertyInfo.PropertyType));
                    }
                    catch (JsonException jex)
                    {
                        // Try to fix by adding quotes, but then fail if that doesn't work
                        logger?.LogWarning(jex, $"Failed to deserialize {propertyName} from {strVal}, trying again with \"{strVal}\"");
                        try
                        {
                            propertyInfo.SetValue(obj, JsonSerializer.Deserialize($"\"{strVal}\"", propertyInfo.PropertyType));
                        }
                        catch (Exception ex)
                        {
                            logger?.LogWarning(jex, $"Failed to deserialize {propertyName} from \"{strVal}\"");
                        }
                    }
                }
            }
        }

        return obj;
    }
}