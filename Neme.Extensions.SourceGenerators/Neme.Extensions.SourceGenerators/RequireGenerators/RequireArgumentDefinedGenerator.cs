using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal sealed class RequireArgumentDefinedGenerator(INamedTypeSymbol enumType) : IRequireGenerator
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
            {
                textWriter.WriteLine($"public static void ArgumentDefined(");

                using (textWriter.WithIndent(baselineIndent + 1))
                {
                    textWriter.WriteLine($"{enumTypeName} argument,");
                    textWriter.WriteLine($"string? message = null,");
                    textWriter.WriteLine($"[global::System.Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)");
                }

                textWriter.WriteLine('{');

                using (textWriter.WithIndent(baselineIndent + 1))
                {
                    textWriter.WriteLine($"if (!{extensionClassName}.IsDefined(argument))");

                    using (textWriter.WithIndent(baselineIndent + 2))
                        textWriter.WriteLine($"Throw(global::Neme.Extensions.Contracts.Assert.NotNull(paramName), argument, message);");

                    textWriter.WriteLine();
                    textWriter.WriteLine("static void Throw(string? paramName, object? actualValue, string? message) =>");

                    using (textWriter.WithIndent(baselineIndent + 2))
                        textWriter.WriteLine($"throw new global::Neme.Extensions.ArgumentException2(paramName, actualValue, message ?? \"Argument is not defined.\");");
                }

                textWriter.WriteLine('}');
            }

            return textWriter.InnerWriter.ToString();
        }
    }
}
