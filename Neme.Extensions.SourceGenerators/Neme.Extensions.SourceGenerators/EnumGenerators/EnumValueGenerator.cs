using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumValueGenerator
{
    private static readonly SymbolDisplayFormat s_keywordFormat = new(
    SymbolDisplayGlobalNamespaceStyle.Included,
    SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
    SymbolDisplayGenericsOptions.None,
    SymbolDisplayMemberOptions.IncludeContainingType,
    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

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

        var typeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = underlyingType.ToDisplayString(s_keywordFormat);

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public {underlyingTypeName} Value =>");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"({underlyingTypeName})value;");
            textWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return textWriter.InnerWriter.ToString();
        }
    }
}

