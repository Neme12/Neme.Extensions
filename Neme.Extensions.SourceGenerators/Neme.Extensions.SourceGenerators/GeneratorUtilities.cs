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

    public static bool HasConflictingFieldOrProperty(
        Compilation compilation,
        INamedTypeSymbol type,
        string propertyName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return type.GetMembers(propertyName).Any(static member => member is IFieldSymbol or IPropertySymbol);
    }
}
