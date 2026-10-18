using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal sealed class RequireValueDefinedGenerator(INamedTypeSymbol enumType) : IRequireGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var extensionClassName = EnumExtensionsGenerator.GetFullyQualifiedExtensionsClassName(_enumType);

        textWriter.WriteLine($"public static void ValueDefined(");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine($"{enumTypeName} value,");
            textWriter.WriteLine($"string? message = null,");
            textWriter.WriteLine($"[global::System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? valueSource = null)");
        }

        textWriter.WriteLine('{');

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine($"if (!{extensionClassName}.IsDefined(value))");

            using (textWriter.AddIndent())
                textWriter.WriteLine($"Throw(value, message, global::Neme.Extensions.Contracts.Assert.NotNull(valueSource));");

            textWriter.WriteLine();
            textWriter.WriteLine("static void Throw(object? value, string? message, string? valueSource) =>");

            using (textWriter.AddIndent())
                textWriter.WriteLine($@"throw new Neme.Extensions.Contracts.InvalidValueException(valueSource!, value, message ?? ""Value is not defined."");");
        }

        textWriter.WriteLine('}');

        return true;
    }
}
