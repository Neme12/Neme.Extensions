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

    public static string? GetUnsignedEnumTypeName(ITypeSymbol underlyingType)
    {
        return underlyingType.SpecialType switch
        {
            SpecialType.System_SByte => "byte",
            SpecialType.System_Byte => "byte",
            SpecialType.System_Int16 => "ushort",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_Int32 => "uint",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_Int64 => "ulong",
            SpecialType.System_UInt64 => "ulong",
            _ => null,
        };
    }

    public static string? GetUnsignedEnumValueExpression(ITypeSymbol underlyingType, string valueExpression)
    {
        return underlyingType.SpecialType switch
        {
            SpecialType.System_SByte => $"unchecked((byte)(sbyte){valueExpression})",
            SpecialType.System_Byte => $"unchecked((byte){valueExpression})",
            SpecialType.System_Int16 => $"unchecked((ushort)(short){valueExpression})",
            SpecialType.System_UInt16 => $"unchecked((ushort){valueExpression})",
            SpecialType.System_Int32 => $"unchecked((uint)(int){valueExpression})",
            SpecialType.System_UInt32 => $"unchecked((uint){valueExpression})",
            SpecialType.System_Int64 => $"unchecked((ulong)(long){valueExpression})",
            SpecialType.System_UInt64 => $"unchecked((ulong){valueExpression})",
            _ => null,
        };
    }

    public static string? GetBitOperationsPow2ValueExpression(ITypeSymbol underlyingType, string valueExpression)
    {
        return underlyingType.SpecialType switch
        {
            SpecialType.System_SByte => $"unchecked((uint)(byte)(sbyte){valueExpression})",
            SpecialType.System_Byte => $"unchecked((uint)(byte){valueExpression})",
            SpecialType.System_Int16 => $"unchecked((uint)(ushort)(short){valueExpression})",
            SpecialType.System_UInt16 => $"unchecked((uint)(ushort){valueExpression})",
            SpecialType.System_Int32 => $"unchecked((uint)(int){valueExpression})",
            SpecialType.System_UInt32 => $"unchecked((uint){valueExpression})",
            SpecialType.System_Int64 => $"unchecked((ulong)(long){valueExpression})",
            SpecialType.System_UInt64 => $"unchecked((ulong){valueExpression})",
            _ => null,
        };
    }

    public static ImmutableArray<IFieldSymbol> GetEnumMembers(INamedTypeSymbol enumType, CancellationToken cancellationToken)
    {
        // Do not use LINQ for perf
        cancellationToken.ThrowIfCancellationRequested();

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
        // Do not use LINQ for perf
        cancellationToken.ThrowIfCancellationRequested();

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
