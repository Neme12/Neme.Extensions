using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.EnumGenerators;
using System.Text;

namespace Neme.Extensions.SourceGenerators.Tests;

internal static class TestUtilities
{
    private static readonly SymbolDisplayFormat s_keywordTypeFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Included,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        SymbolDisplayGenericsOptions.None,
        SymbolDisplayMemberOptions.IncludeContainingType,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    public static EnumGenerationContext CreateEnumGenerationContext(
        Compilation compilation,
        INamedTypeSymbol enumType)
    {
        var isFlagsEnum = enumType.GetAttributes().Any(static attribute => attribute.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");
        var enumMembers = enumType!.GetMembers().OfType<IFieldSymbol>().ToImmutableArray();
        var constantMembers = enumMembers.Where(static member => member.HasConstantValue).ToImmutableArray();

        return new EnumGenerationContext
        {
            Compilation = compilation,
            EnumType = enumType,
            EnumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            IsFlagsEnum = isFlagsEnum,
            EnumMembers = enumMembers,
            ConstantEnumMembers = constantMembers,
            UnderlyingTypeName = enumType.EnumUnderlyingType?.ToDisplayString(s_keywordTypeFormat),
        };
    }

    public static string NormalizeLineEndings(string value)
    {
        var normalized = value.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();
        var lines = normalized.Split('\n');
        var builder = new StringBuilder(normalized.Length);

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimEnd();
            builder.Append(trimmed);

            if (i < lines.Length - 1)
                builder.Append('\n');
        }

        return builder.ToString();
    }
}
