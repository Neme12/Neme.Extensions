using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumStaticIsDefinedGenerator
{
    public static string GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = enumType
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(static field => field.HasConstantValue)
            .ToImmutableArray();

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
