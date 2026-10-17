using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

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

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;

            if (definedPatterns.Count == 0)
                textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) => false;");
            else
            {
                textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) =>");
                textWriter.Indent = baselineIndent + 1;
                textWriter.WriteLine("value is");
                textWriter.Indent = baselineIndent + 2;

                for (var i = 0; i < definedPatterns.Count; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var suffix = i == definedPatterns.Count - 1 ? ";" : " or";
                    textWriter.WriteLine($"{definedPatterns[i]}{suffix}");
                }
            }

            return textWriter.InnerWriter.ToString();
        }
    }
}
