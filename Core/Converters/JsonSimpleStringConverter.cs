using System;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace CKAN
{
    /// <summary>
    /// Serialises things that can be converted
    /// to simple strings and back.
    /// </summary>
    public class JsonSimpleStringConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType is JsonTokenType.String)
                {
                    var myObject = Activator.CreateInstance(typeToConvert, reader.GetString());
                    return (T?)myObject;
                }
                if (reader.TokenType is JsonTokenType.Number)
                {
                    var isLong = reader.TryGetInt64(out var longNum);
                    if (isLong)
                    {
                        var myObject = Activator.CreateInstance(typeToConvert, longNum.ToString());
                        return (T?)myObject;
                    }
                    var isDouble = reader.TryGetDouble(out var doubleNum);
                    if (isDouble)
                    {
                        var myObject = Activator.CreateInstance(typeToConvert, doubleNum.ToString());
                        return (T?)myObject;
                    }

                    return default;
                    //var myObject = Activator.CreateInstance(typeToConvert, reader.GetInt64().ToString());
                    //return (T?)myObject;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
}
