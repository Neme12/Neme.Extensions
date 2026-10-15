using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumStaticNoneGenerator
{
    public static string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumMembers = enumType.GetMembers()
            .OfType<IFieldSymbol>()
            .ToImmutableArray();

        if (enumMembers.Any(static member => member.Name == "None"))
            return null;

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public static {enumTypeName} None =>");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("default;");

            return textWriter.InnerWriter.ToString();
        }
    }
}
