using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumStaticAllGenerator
{
    public static string GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = enumType
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(static field => field.HasConstantValue)
            .ToImmutableArray();

        var allParts = new string[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allParts[i] = $"{enumTypeName}.{enumMembers[i].Name}";
        }

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;

            if (allParts.Length == 0)
                textWriter.WriteLine($"public static {enumTypeName} All => default;");
            else
            {
                textWriter.WriteLine($"public static {enumTypeName} All =>");
                textWriter.Indent = baselineIndent + 1;

                for (var i = 0; i < allParts.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var suffix = i == allParts.Length - 1 ? ";" : " |";
                    textWriter.WriteLine($"{allParts[i]}{suffix}");
                }
            }

            return textWriter.InnerWriter.ToString();
        }
    }
}
