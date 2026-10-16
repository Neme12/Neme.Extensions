using Microsoft.CodeAnalysis;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal interface IEnumGenerator
{
    public string? GetTypeMemberSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken);

    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken);
}
