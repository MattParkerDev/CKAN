using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CKAN.Versioning;

namespace CKAN;

public class DictionaryWithErrorHandlingConverter : JsonConverter<SortedDictionary<ModuleVersion, CkanModule>>
{
    //private static int _runs = 0;
    public override SortedDictionary<ModuleVersion, CkanModule> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var concurrentDict = new ConcurrentDictionary<ModuleVersion, CkanModule>();

        // Open the JSON object
        using var doc = JsonDocument.ParseValue(ref reader);
        Parallel.ForEach(doc.RootElement.EnumerateObject(), property =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(property.Name))
                {
                    return;
                }

                var moduleVersion = new ModuleVersion(property.Name);
                var value = property.Value.Deserialize<CkanModule>(options);

                concurrentDict[moduleVersion] = value!;
            }
            catch (Exception e)
            {
                // If deserialization fails for a pair, simply ignore it
                Console.WriteLine($"Failed to deserialize key-value pair: {e.Message}");
            }
        });

        var sortedDictionary = new SortedDictionary<ModuleVersion, CkanModule>(concurrentDict);
        return sortedDictionary;
    }

    public override void Write(Utf8JsonWriter writer, SortedDictionary<ModuleVersion, CkanModule> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            JsonSerializer.Serialize(writer, kvp.Key, options);
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }
        writer.WriteEndObject();
    }
}
