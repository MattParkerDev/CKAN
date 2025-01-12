using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKAN
{
    public class JsonRelationshipConverter : JsonConverter<List<RelationshipDescriptor>>
    {
        // public override bool CanConvert(Type object_type)
        // {
        //     // Only convert when we're an explicit attribute
        //     return false;
        // }
        
        public override void Write(Utf8JsonWriter writer, List<RelationshipDescriptor> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override List<RelationshipDescriptor>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                return null;
            }

            var rels = new List<RelationshipDescriptor>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                using (var jsonDocument = JsonDocument.ParseValue(ref reader))
                {
                    var jsonObject = jsonDocument.RootElement;

                    if (jsonObject.TryGetProperty("any_of", out _))
                    {
                        // Catch confused/invalid metadata
                        foreach (string forbiddenPropertyName in AnyOfRelationshipDescriptor.ForbiddenPropertyNames)
                        {
                            if (jsonObject.TryGetProperty(forbiddenPropertyName, out _))
                            {
                                throw new Kraken(string.Format(
                                    Properties.Resources.JsonRelationshipConverterAnyOfCombined, forbiddenPropertyName));
                            }
                        }

                        var anyOfDescriptor = JsonSerializer.Deserialize<AnyOfRelationshipDescriptor>(jsonObject.GetRawText(), options);
                        if (anyOfDescriptor != null)
                        {
                            rels.Add(anyOfDescriptor);
                        }
                    }
                    else if (jsonObject.TryGetProperty("name", out _))
                    {
                        var moduleDescriptor = JsonSerializer.Deserialize<ModuleRelationshipDescriptor>(jsonObject.GetRawText(), options);
                        if (moduleDescriptor != null)
                        {
                            rels.Add(moduleDescriptor);
                        }
                    }
                }
            }

            return rels;
        }
    }
}
