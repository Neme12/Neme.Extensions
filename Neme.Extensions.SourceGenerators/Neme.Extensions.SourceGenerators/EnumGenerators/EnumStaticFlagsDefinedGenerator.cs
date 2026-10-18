using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticFlagsDefinedGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumUnderlyingType = _enumType.EnumUnderlyingType;
        if (enumUnderlyingType is null)
            return false;

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = enumUnderlyingType.ToDisplayString(GeneratorUtilities.KeywordTypeFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        var allFlagsParts = new string[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allFlagsParts[i] = Invariant($"""({underlyingTypeName}){enumTypeName}.{enumMembers[i].Name}""");
        }

        textWriter.WriteLine($"public static bool FlagsDefined({enumTypeName} value)");
        textWriter.WriteLine("{");

        using (textWriter.AddIndent())
        {
            if (allFlagsParts.Length == 0)
                textWriter.WriteLine($"{underlyingTypeName} allFlags = default;");
            else
                textWriter.WriteLine($"{underlyingTypeName} allFlags =");

            if (allFlagsParts.Length > 0)
            {
                using (textWriter.AddIndent())
                {
                    for (var i = 0; i < allFlagsParts.Length; ++i)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var suffix = i == allFlagsParts.Length - 1 ? ";" : " |";
                        textWriter.WriteLine($"{allFlagsParts[i]}{suffix}");
                    }
                }
            }

            textWriter.WriteLine();
            textWriter.WriteLine($"return ((({underlyingTypeName})value) | allFlags) == allFlags;");
        }

        textWriter.WriteLine("}");

        return true;
    }
}
