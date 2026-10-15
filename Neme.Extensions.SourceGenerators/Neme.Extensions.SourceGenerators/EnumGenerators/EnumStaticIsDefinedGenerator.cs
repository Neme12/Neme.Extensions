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

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public static bool IsDefined({enumTypeName} value) =>");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("value switch");
            textWriter.WriteLine("{");
            textWriter.Indent = baselineIndent + 2;

            var seenValues = new HashSet<object>();
            foreach (var member in enumMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!seenValues.Add(member.ConstantValue!))
                    continue;

                textWriter.WriteLine($"{enumTypeName}.{member.Name} => true,");
            }

            textWriter.WriteLine("_ => false,");

            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("};");

            return textWriter.InnerWriter.ToString();
        }
    }
}
