using Microsoft.CodeAnalysis;

namespace Neme.Extensions.SourceGenerators;

internal static class GeneratorUtilities
{
    public static readonly SymbolDisplayFormat KeywordTypeFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Included,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        SymbolDisplayGenericsOptions.None,
        SymbolDisplayMemberOptions.IncludeContainingType,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
}
