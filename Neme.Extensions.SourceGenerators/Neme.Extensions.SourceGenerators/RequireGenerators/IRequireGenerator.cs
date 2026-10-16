using Microsoft.CodeAnalysis;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal interface IRequireGenerator
{
    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken);
}
