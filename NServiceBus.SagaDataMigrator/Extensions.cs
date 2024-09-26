using System.Reflection;

namespace NServiceBus.SagaDataMigrator;

public static class Extensions
{
    public static void CopyFrom<T>(this T destination, T source) where T : IContainSagaData
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanWrite && x.CanRead && x.Name != "Id")
            .ToList();
        foreach (var prop in properties)
            prop.SetValue(destination, prop.GetValue(source));
        
    }
}