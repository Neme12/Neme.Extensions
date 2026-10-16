using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal static class RequireArgumentDefinedGenerator
{
    public static string GetSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public static void ArgumentDefined(");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"{enumTypeName} argument,");
            textWriter.WriteLine($"string? message = null,");
            textWriter.WriteLine($"[global::System.Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine('{');
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"if (!{enumTypeName}Extensions.IsDefined(argument))");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine($"Throw(paramName, argument, message);");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine();
            textWriter.WriteLine("static void Throw(string? paramName, object? actualValue, string? message) =>");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine($"throw new global::Neme.Extensions.ArgumentException2(paramName, actualValue, message);");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine('}');
            return stringWriter.ToString();
        }
    }
}
