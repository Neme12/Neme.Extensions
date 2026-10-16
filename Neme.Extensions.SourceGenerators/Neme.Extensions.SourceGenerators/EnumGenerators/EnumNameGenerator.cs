using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumNameGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumMembers = _enumType
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
