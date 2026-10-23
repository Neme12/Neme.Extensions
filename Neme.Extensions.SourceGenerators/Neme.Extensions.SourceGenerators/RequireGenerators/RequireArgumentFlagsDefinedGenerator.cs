using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal sealed class RequireArgumentFlagsDefinedGenerator(RequireGenerationContext generationContext) : IRequireGenerator
{
    private readonly string _enumTypeName = generationContext.EnumTypeName;
    private readonly string _extensionClassName = generationContext.ExtensionClassName;

    public bool EmitExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        textWriter.WriteLine($"public static void ArgumentFlagsDefined(");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine($"{_enumTypeName} argument,");
            textWriter.WriteLine($"string? message = null,");
            textWriter.WriteLine($"[global::System.Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)");
        }

        textWriter.WriteLine('{');

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine($"if (!{_extensionClassName}.FlagsDefined(argument))");

            using (textWriter.AddIndent())
                textWriter.WriteLine($"Throw(global::Neme.Extensions.Contracts.Assert.NotNull(paramName), argument, message);");

            textWriter.WriteLine();
            textWriter.WriteLine("static void Throw(string? paramName, object? actualValue, string? message) =>");

            using (textWriter.AddIndent())
                textWriter.WriteLine($"throw new global::Neme.Extensions.ArgumentException2(paramName, actualValue, message ?? \"The argument contains undefined flags.\");");
        }

        textWriter.WriteLine('}');

        return true;
    }
}
