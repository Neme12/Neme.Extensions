using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumIncludesGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = generationContext.EnumType;
    private readonly string _enumTypeName = generationContext.EnumTypeName;

    public bool EmitTypeSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool EmitExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumUnderlyingType = _enumType.EnumUnderlyingType;
        if (enumUnderlyingType is null)
            return false;

        var typeName = _enumTypeName;

        var isPow2Operand = GeneratorUtilities.GetBitOperationsPow2ValueExpression(enumUnderlyingType, "flag");
        if (isPow2Operand is null)
            return false;

        textWriter.WriteLine($"public bool Includes({typeName} flag)");
        textWriter.WriteLine('{');

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine($"if (!global::System.Numerics.BitOperations.IsPow2({isPow2Operand}))");

            using (textWriter.AddIndent())
                textWriter.WriteLine("ThrowNotOneFlag(flag);");

            textWriter.WriteLine();
            textWriter.WriteLine("return (@enum & flag) != 0;");
            textWriter.WriteLine();
            textWriter.WriteLine("[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]");
            textWriter.WriteLine($"static void ThrowNotOneFlag({typeName} flag) =>");

            using (textWriter.AddIndent())
                textWriter.WriteLine($"throw new global::System.ArgumentException($\"The value '{{flag}}' is not a single flag value for enum type '{{nameof({_enumType})}}'.\", nameof(flag));");
        }

        textWriter.WriteLine('}');

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}

