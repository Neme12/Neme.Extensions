using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticFlagsDefinedGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumUnderlyingType = _enumType.EnumUnderlyingType;
        if (enumUnderlyingType is null)
            return null;

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = enumUnderlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        var allFlagsParts = new string[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allFlagsParts[i] = $"({underlyingTypeName}){enumTypeName}.{enumMembers[i].Name}";
        }

        using (var textWriter = IndentedTextWriter.CreateNew())
        {
            using (textWriter.WithIndent(baselineIndent))
            {
                textWriter.WriteLine($"public static bool FlagsDefined({enumTypeName} value)");
                textWriter.WriteLine("{");
            }

            using (textWriter.WithIndent(baselineIndent + 1))
            {
                if (allFlagsParts.Length == 0)
                    textWriter.WriteLine($"{underlyingTypeName} allFlags = default;");
                else
                    textWriter.WriteLine($"{underlyingTypeName} allFlags =");
            }

            if (allFlagsParts.Length > 0)
            {
                using (textWriter.WithIndent(baselineIndent + 2))
                {
                    for (var i = 0; i < allFlagsParts.Length; ++i)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var suffix = i == allFlagsParts.Length - 1 ? ";" : " |";
                        textWriter.WriteLine($"{allFlagsParts[i]}{suffix}");
                    }
                }
            }

            using (textWriter.WithIndent(baselineIndent + 1))
            {
                textWriter.WriteLine();
                textWriter.WriteLine($"return ((({underlyingTypeName})value) | allFlags) == allFlags;");
            }

            using (textWriter.WithIndent(baselineIndent))
                textWriter.WriteLine("}");

            return textWriter.InnerWriter.ToString();
        }
    }
}
