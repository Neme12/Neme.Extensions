using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

#if NET8_0_OR_GREATER
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
#else
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
#endif
[JsonSerializable(typeof(FileCacheMetadata))]
internal partial class FileCacheJsonSerializerContext : JsonSerializerContext
{
}
