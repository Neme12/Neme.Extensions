using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumStaticUnderlyingValuesGenerator
{
    public static string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var underlyingType = enumType.EnumUnderlyingType;
        if (underlyingType is null)
            return null;

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = underlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        var enumMembers = enumType
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(static field => field.HasConstantValue)
            .ToImmutableArray();

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
