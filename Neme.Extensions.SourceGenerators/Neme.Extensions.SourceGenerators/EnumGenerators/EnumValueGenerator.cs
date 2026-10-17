using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumValueGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public string? GetTypeMemberSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        return null;
    }

    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var underlyingType = _enumType.EnumUnderlyingType;
        if (underlyingType is null)
            return null;

        var underlyingTypeName = underlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        using (var textWriter = IndentedTextWriter.CreateNew())
        {
            using (textWriter.WithIndent(baselineIndent))
            {
                textWriter.WriteLine($"public {underlyingTypeName} Value =>");

                using (textWriter.WithIndent(baselineIndent + 1))
                    textWriter.WriteLine($"({underlyingTypeName})@enum;");
            }

            textWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return textWriter.InnerWriter.ToString();
        }
    }
}

