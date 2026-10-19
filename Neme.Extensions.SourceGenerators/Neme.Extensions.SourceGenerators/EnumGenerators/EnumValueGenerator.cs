using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumValueGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly string? _underlyingTypeName = generationContext.UnderlyingTypeName;

    public bool EmitTypeSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool EmitExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_underlyingTypeName is null)
            return false;

        textWriter.WriteLine($"public {_underlyingTypeName} Value =>");

        using (textWriter.AddIndent())
            textWriter.WriteLine($"({_underlyingTypeName})@enum;");

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}

