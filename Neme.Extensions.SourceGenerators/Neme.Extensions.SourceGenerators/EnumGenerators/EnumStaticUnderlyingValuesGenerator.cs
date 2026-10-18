using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticUnderlyingValuesGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var underlyingType = _enumType.EnumUnderlyingType;
        if (underlyingType is null)
            return false;

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = underlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        textWriter.WriteLine($"public static global::System.ReadOnlySpan<{underlyingTypeName}> UnderlyingValues =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("[");

            using (textWriter.AddIndent())
            {
                foreach (var member in enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"({underlyingTypeName}){enumTypeName}.{member.Name},");
                }
            }

            textWriter.WriteLine("];");
        }

        return true;
    }
}
