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
        /// <summary>
        /// We *only* want to be triggered for types that have explicitly
        /// set an attribute in their class saying they can be converted.
        /// By returning false here, we declare we're not interested in participating
        /// in any other conversions.
        /// </summary>
        /// <returns>
        /// false
        /// </returns>
        // public override bool CanConvert(Type objectType)
        // {
        //     return false;
        // }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return (T?)Activator.CreateInstance(typeof(T), reader.GetString());
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
}
