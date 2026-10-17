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

    public static ImmutableArray<IFieldSymbol> GetEnumMembers(INamedTypeSymbol enumType, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Do not use LINQ for perf
        var members = enumType.GetMembers();
        var resultBuilder = ImmutableArray.CreateBuilder<IFieldSymbol>(members.Length);

        foreach (var member in members)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (member is IFieldSymbol field)
                resultBuilder.Add(field);
        }

        return resultBuilder.DrainToImmutable();

    }

    public static ImmutableArray<IFieldSymbol> GetEnumMembersWithConstantValue(INamedTypeSymbol enumType, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Do not use LINQ for perf
        var members = enumType.GetMembers();
        var resultBuilder = ImmutableArray.CreateBuilder<IFieldSymbol>(members.Length);

        foreach (var member in members)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (member is IFieldSymbol { HasConstantValue: true } field)
                resultBuilder.Add(field);
        }

        return resultBuilder.DrainToImmutable();

    }
}
