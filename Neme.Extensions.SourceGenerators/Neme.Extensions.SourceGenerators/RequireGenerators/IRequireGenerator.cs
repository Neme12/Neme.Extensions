using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal interface IRequireGenerator
{
    public bool EmitExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken);
}
