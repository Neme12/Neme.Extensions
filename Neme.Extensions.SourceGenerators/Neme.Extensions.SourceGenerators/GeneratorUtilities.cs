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
        // Do not use LINQ for perf
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var member in type.GetMembers(propertyName))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (member is IFieldSymbol or IPropertySymbol)
                return true;
        }

        return false;
    }

    public static ImmutableArray<IFieldSymbol> GetEnumMembers(INamedTypeSymbol enumType, CancellationToken cancellationToken)
    {
        // Do not use LINQ for perf
        cancellationToken.ThrowIfCancellationRequested();

        var resultBuilder = ImmutableArray.CreateBuilder<IFieldSymbol>(enumType.GetMembers().Length);

        foreach (var member in enumType.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (member is IFieldSymbol field)
                resultBuilder.Add(field);
        }

        return resultBuilder.DrainToImmutable();

    }

    public static ImmutableArray<IFieldSymbol> GetEnumMembersWithConstantValue(INamedTypeSymbol enumType, CancellationToken cancellationToken)
    {
        // Do not use LINQ for perf
        cancellationToken.ThrowIfCancellationRequested();

        var resultBuilder = ImmutableArray.CreateBuilder<IFieldSymbol>(enumType.GetMembers().Length);

        foreach (var member in enumType.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (member is IFieldSymbol { HasConstantValue: true } field)
                resultBuilder.Add(field);
        }

        return resultBuilder.DrainToImmutable();

    }
}
