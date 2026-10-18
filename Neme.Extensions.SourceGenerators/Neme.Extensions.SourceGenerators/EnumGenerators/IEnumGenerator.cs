using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal interface IEnumGenerator
{
    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken);

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken);
}
