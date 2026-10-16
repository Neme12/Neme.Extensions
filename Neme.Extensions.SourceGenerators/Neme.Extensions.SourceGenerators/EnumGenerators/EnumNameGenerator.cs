using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumNameGenerator
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

        cancellationToken.ThrowIfCancellationRequested();

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var switchWriter = new IndentedTextWriter(stringWriter))
        {
            switchWriter.Indent = baselineIndent;
            switchWriter.WriteLine("public string? Name =>");
            switchWriter.Indent = baselineIndent + 1;
            switchWriter.WriteLine("@enum switch");
            switchWriter.WriteLine("{");
            switchWriter.Indent = baselineIndent + 2;

            var seenValues = new HashSet<object>();

            foreach (var member in enumMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var constantValue = member.ConstantValue;
                if (constantValue is null || !seenValues.Add(constantValue))
                    continue;

                switchWriter.WriteLine($"{enumTypeName}.{member.Name} => nameof({enumTypeName}.{member.Name}),");
            }

            switchWriter.WriteLine("_ => global::System.Enum.GetName(@enum),");

            switchWriter.Indent = baselineIndent + 1;
            switchWriter.WriteLine("};");

            switchWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return switchWriter.InnerWriter.ToString();
        }
    }
}
