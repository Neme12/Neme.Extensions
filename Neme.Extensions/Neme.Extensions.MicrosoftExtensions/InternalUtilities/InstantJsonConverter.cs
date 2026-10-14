using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Text;

namespace Neme.Extensions.MicrosoftExtensions.InternalUtilities;

internal sealed class InstantJsonConverter : JsonConverter<Instant>
{
    private static readonly InstantPattern s_pattern = InstantPattern.General;

    public override Instant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString();
        if (text is null)
            throw new JsonException("Cannot deserialize null as Instant");

        var parseResult = s_pattern.Parse(text);
        if (!parseResult.Success)
            throw new JsonException($"Failed to parse Instant: {parseResult.Exception?.Message}");

        return parseResult.Value;
    }

    public override void Write(Utf8JsonWriter writer, Instant value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(s_pattern.Format(value));
    }
}
