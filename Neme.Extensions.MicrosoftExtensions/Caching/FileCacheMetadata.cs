using System.Text.Json.Serialization;
using Neme.Extensions.MicrosoftExtensions.InternalUtilities;
using NodaTime;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

internal readonly record struct FileCacheMetadata
{
#pragma warning disable SYSLIB1037
    [JsonConverter(typeof(InstantJsonConverter))]
    public Instant ExpiresAt { get; init; }
#pragma warning restore SYSLIB1037

    [JsonConverter(typeof(NullableDurationJsonConverter))]
    public Duration? SlidingExpiration { get; init; }
}
