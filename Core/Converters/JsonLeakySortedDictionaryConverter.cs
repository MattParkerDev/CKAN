using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using log4net;

namespace CKAN;

/// <summary>
/// [De]serializes a dictionary that might have some questionably
/// valid data in it.
/// If exceptions are thrown for any key/value pair, leave it out.
/// Removes CkanModule objects from AvailableModule.module_version
/// if License throws BadMetadataKraken.
/// </summary>
public class JsonLeakySortedDictionaryConverter<K, V> : JsonConverter<SortedDictionary<K, V>> 
    where K : class 
    where V : class
{
    public override SortedDictionary<K, V>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dict = new SortedDictionary<K, V>();
        
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected a JSON object.");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dict;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected a property name.");
            }

            var keyString = reader.GetString();
            K? key = null;

            try
            {
                key = (K?)Activator.CreateInstance(typeof(K), keyString);
            }
            catch (Exception exc)
            {
                LogWarn($"Failed to create key instance for {keyString}", exc);
                reader.TrySkip();
                continue;
            }

            reader.Read();
            V? value = null;
            
            if (reader.TokenType is not JsonTokenType.StartObject)
            {
                LogWarn($"Expected an object for key {keyString}, got {reader.TokenType}", new JsonException());
                reader.TrySkip();
                continue;
            }

            try
            {
                value = JsonSerializer.Deserialize<V>(ref reader, options);
            }
            catch (Exception exc)
            {
                LogWarn($"Failed to deserialize value for key {keyString}", exc);
                continue;
            }

            if (key != null && value != null)
            {
                dict[key] = value;
            }
        }

        throw new JsonException("Unexpected end of JSON.");
    }

    public override void Write(Utf8JsonWriter writer, SortedDictionary<K, V> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type typeToConvert)
    {
        var canConvert = typeToConvert == typeof(SortedDictionary<K, V>);
        return canConvert;
    }

    private static void LogWarn(string message, Exception exc)
    {
        // Replace with your preferred logging mechanism
        Console.WriteLine($"{message}: {exc.Message}");
    }
}