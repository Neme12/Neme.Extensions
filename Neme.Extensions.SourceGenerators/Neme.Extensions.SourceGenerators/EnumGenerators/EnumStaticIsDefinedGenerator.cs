using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticIsDefinedGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        var seenValues = new HashSet<object>();
        var definedPatterns = new List<string>(enumMembers.Length);
        foreach (var member in enumMembers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!seenValues.Add(member.ConstantValue!))
                continue;

            definedPatterns.Add(Invariant($"""{enumTypeName}.{member.Name}"""));
        }

        if (definedPatterns.Count == 0)
            textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) => false;");
        else
        {
            textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) =>");

            using (textWriter.AddIndent())
            {
                textWriter.WriteLine("value is");

                using (textWriter.AddIndent())
                {
                    for (var i = 0; i < definedPatterns.Count; ++i)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var suffix = i == definedPatterns.Count - 1 ? ";" : " or";
                        textWriter.WriteLine($"{definedPatterns[i]}{suffix}");
                    }
                }
            }
        }

        return true;
    }
}
