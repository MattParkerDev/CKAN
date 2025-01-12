using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKAN
{
    /// <summary>
    /// With thanks to
    /// https://stackoverflow.com/questions/18994685/how-to-handle-both-a-single-item-and-an-array-for-the-same-property-using-json-n
    /// </summary>
    public class JsonSingleOrArrayConverterNew<T> : JsonConverter<List<T>>
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
        //public override bool CanConvert(Type object_type) => false;

        public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // read as an array, or as a single item
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<T>>(ref reader, options); // This looks like rubbish
            }
            else
            {
                return [JsonSerializer.Deserialize<T>(ref reader, options)!];
            }
        }
        
        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            // write as an array, or as a single item
            if (value.Count == 1)
            {
                writer.WriteStringValue(value[0]!.ToString());
            }
            else
            {
                writer.WriteStartArray();
                foreach (var item in value)
                {
                    writer.WriteStringValue(item!.ToString());
                }
                writer.WriteEndArray();
            }
        }
    }
}
