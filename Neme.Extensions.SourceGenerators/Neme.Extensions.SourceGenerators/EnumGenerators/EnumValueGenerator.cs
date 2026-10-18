using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumValueGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var underlyingType = _enumType.EnumUnderlyingType;
        if (underlyingType is null)
            return false;

        var underlyingTypeName = underlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        textWriter.WriteLine($"public {underlyingTypeName} Value =>");

        using (textWriter.AddIndent())
            textWriter.WriteLine($"({underlyingTypeName})@enum;");

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}

