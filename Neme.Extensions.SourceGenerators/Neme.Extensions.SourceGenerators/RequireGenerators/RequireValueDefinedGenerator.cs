using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal sealed class RequireValueDefinedGenerator(INamedTypeSymbol enumType) : IRequireGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var extensionClassName = EnumExtensionsGenerator.GetFullyQualifiedExtensionsClassName(_enumType);

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            textWriter.WriteLine($"public static void ValueDefined(");

            using (textWriter.WithAddedIndent())
            {
                textWriter.WriteLine($"{enumTypeName} value,");
                textWriter.WriteLine($"string? message = null,");
                textWriter.WriteLine($"[global::System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? valueSource = null)");
            }

            textWriter.WriteLine('{');

            using (textWriter.WithAddedIndent())
            {
                textWriter.WriteLine($"if (!{extensionClassName}.IsDefined(value))");

                using (textWriter.WithAddedIndent())
                    textWriter.WriteLine($"Throw(value, message, global::Neme.Extensions.Contracts.Assert.NotNull(valueSource));");

                textWriter.WriteLine();
                textWriter.WriteLine("static void Throw(object? value, string? message, string? valueSource) =>");

                using (textWriter.WithAddedIndent())
                    textWriter.WriteLine($@"throw new Neme.Extensions.Contracts.InvalidValueException(valueSource!, value, message ?? ""Value is not defined."");");
            }

            textWriter.WriteLine('}');

            return textWriter.InnerWriter.ToString();
        }
    }
}
