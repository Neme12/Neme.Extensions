using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

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

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public static global::System.ReadOnlySpan<{underlyingTypeName}> UnderlyingValues =>");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("[");
            textWriter.Indent = baselineIndent + 2;

            foreach (var member in enumMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                textWriter.WriteLine($"({underlyingTypeName}){enumTypeName}.{member.Name},");
            }

            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("];");

            return textWriter.InnerWriter.ToString();
        }
    }
}
