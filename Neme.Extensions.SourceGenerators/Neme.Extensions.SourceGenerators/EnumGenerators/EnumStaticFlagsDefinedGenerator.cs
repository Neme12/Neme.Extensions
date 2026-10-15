using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumStaticFlagsDefinedGenerator
{
    public static string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumUnderlyingType = enumType.EnumUnderlyingType;
        if (enumUnderlyingType is null)
            return null;

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = enumUnderlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        var enumMembers = enumType
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(static field => field.HasConstantValue)
            .ToImmutableArray();

        var allFlagsParts = new string[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allFlagsParts[i] = $"({underlyingTypeName}){enumTypeName}.{enumMembers[i].Name}";
        }

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public static bool FlagsDefined({enumTypeName} value)");
            textWriter.WriteLine("{");
            textWriter.Indent = baselineIndent + 1;

            if (allFlagsParts.Length == 0)
                textWriter.WriteLine($"{underlyingTypeName} allFlags = default;");
            else
            {
                textWriter.WriteLine($"{underlyingTypeName} allFlags =");
                textWriter.Indent = baselineIndent + 2;

                for (var i = 0; i < allFlagsParts.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var suffix = i == allFlagsParts.Length - 1 ? ";" : " |";
                    textWriter.WriteLine($"{allFlagsParts[i]}{suffix}");
                }

                textWriter.Indent = baselineIndent + 1;
            }

            textWriter.WriteLine();
            textWriter.WriteLine($"return ((({underlyingTypeName})value) | allFlags) == allFlags;");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine("}");

            return textWriter.InnerWriter.ToString();
        }
    }
}
