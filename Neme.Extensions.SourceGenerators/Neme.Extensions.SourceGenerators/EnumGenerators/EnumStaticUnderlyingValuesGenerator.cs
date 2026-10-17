using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticUnderlyingValuesGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = underlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            textWriter.WriteLine($"public static global::System.ReadOnlySpan<{underlyingTypeName}> UnderlyingValues =>");

            using (textWriter.AddIndent())
            {
                textWriter.WriteLine("[");

                using (textWriter.AddIndent())
                {
                    foreach (var member in enumMembers)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        textWriter.WriteLine($"({underlyingTypeName}){enumTypeName}.{member.Name},");
                    }
                }

                textWriter.WriteLine("];");
            }

            return textWriter.InnerWriter.ToString();
        }
    }
}
