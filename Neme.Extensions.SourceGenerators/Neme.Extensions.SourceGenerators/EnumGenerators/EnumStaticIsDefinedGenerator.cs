using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticIsDefinedGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var seenValues = new HashSet<object>();
        var definedPatterns = new List<string>(enumMembers.Length);
        foreach (var member in enumMembers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!seenValues.Add(member.ConstantValue!))
                continue;

            definedPatterns.Add($"{enumTypeName}.{member.Name}");
        }

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            if (definedPatterns.Count == 0)
                textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) => false;");
            else
            {
                textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) =>");

                using (textWriter.WithIndent(baselineIndent + 1))
                {
                    textWriter.WriteLine("value is");

                    using (textWriter.WithIndent(baselineIndent + 2))
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

            return textWriter.InnerWriter.ToString();
        }
    }
}
