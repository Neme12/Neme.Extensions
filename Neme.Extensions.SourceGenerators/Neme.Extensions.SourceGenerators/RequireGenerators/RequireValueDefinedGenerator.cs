using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal static class RequireValueDefinedGenerator
{
    public static string GetSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var extensionClassName = EnumExtensionsGenerator.GetFullyQualifiedExtensionsClassName(enumType);

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public static void ValueDefined(");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"{enumTypeName} value,");
            textWriter.WriteLine($"string? message = null,");
            textWriter.WriteLine($"[global::System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? valueSource = null)");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine('{');
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"if (!{extensionClassName}.IsDefined(value))");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine($"Throw(value, message, global::Neme.Extensions.Contracts.Assert.NotNull(valueSource));");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine();
            textWriter.WriteLine("static void Throw(object? value, string? message, string? valueSource) =>");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine($@"throw new Neme.Extensions.Contracts.InvalidValueException(valueSource!, value, message ?? ""Value is not defined."");");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine('}');
            return stringWriter.ToString();
        }
    }
}
