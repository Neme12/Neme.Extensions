using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Text;

namespace Neme.Extensions.MicrosoftExtensions.InternalUtilities;

internal sealed class NullableDurationJsonConverter : JsonConverter<Duration?>
{
    private static readonly DurationPattern s_pattern = DurationPattern.Roundtrip;

    public override Duration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var text = reader.GetString();
        if (text is null)
            return null;

        var parseResult = s_pattern.Parse(text);
        if (!parseResult.Success)
            throw new JsonException($"Failed to parse Duration: {parseResult.Exception?.Message}");

        return parseResult.Value;
    }

    public override void Write(Utf8JsonWriter writer, Duration? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(s_pattern.Format(value.Value));
        }
    }
}
