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

        if (type.GetMembers(propertyName).Any(static member => member is IFieldSymbol or IPropertySymbol))
            return true;

        foreach (var location in type.Locations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!location.IsInSource || location.SourceTree is null)
                continue;

            var semanticModel = compilation.GetSemanticModel(location.SourceTree);
            var position = location.SourceSpan.Start;

            foreach (var symbol in semanticModel.LookupSymbols(position, container: type, name: propertyName, includeReducedExtensionMethods: true))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (symbol is IFieldSymbol or IPropertySymbol &&
                    symbol.Name == propertyName)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
