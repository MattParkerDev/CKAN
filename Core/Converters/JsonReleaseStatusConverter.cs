using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKAN
{
    public class JsonReleaseStatusConverter : JsonConverter<ReleaseStatus>
    {
        public override ReleaseStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            return value switch
            {
                "alpha" => ReleaseStatus.development,
                "beta"  => ReleaseStatus.testing,
                null or "stable"    => ReleaseStatus.stable,
                ""      => throw new JsonException("Empty release_status string"),
                _       => throw new JsonException($"Unexpected value {value} for {nameof(ReleaseStatus)}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, ReleaseStatus value, JsonSerializerOptions options)
        {
            string statusString = value switch
            {
                ReleaseStatus.development => "alpha",
                ReleaseStatus.testing     => "beta",
                ReleaseStatus.stable      => "stable",
                _                         => throw new JsonException($"Unexpected value {value} for {nameof(ReleaseStatus)}"),
            };

            writer.WriteStringValue(statusString);
        }
    }
}