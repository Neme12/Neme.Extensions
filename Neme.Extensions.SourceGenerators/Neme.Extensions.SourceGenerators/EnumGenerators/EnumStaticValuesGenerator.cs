using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticValuesGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        using (var textWriter = IndentedTextWriter.CreateNew())
        {
            using (textWriter.WithIndent(baselineIndent))
                textWriter.WriteLine($"public static global::System.ReadOnlySpan<{enumTypeName}> Values =>");

            using (textWriter.WithIndent(baselineIndent + 1))
                textWriter.WriteLine("[");

            using (textWriter.WithIndent(baselineIndent + 2))
            {
                foreach (var member in enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"{enumTypeName}.{member.Name},");
                }
            }

            using (textWriter.WithIndent(baselineIndent + 1))
                textWriter.WriteLine("];");

            return textWriter.InnerWriter.ToString();
        }
    }
}
