using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumIncludesGenerator(INamedTypeSymbol enumType, string enumTypeName) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly string _enumTypeName = enumTypeName;

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumUnderlyingType = _enumType.EnumUnderlyingType;
        if (enumUnderlyingType is null)
            return false;

        var typeName = _enumTypeName;

        // Widen the value to uint or ulong as IsPow2 only has overloads for those
        var isPow2Operand = enumUnderlyingType.SpecialType switch
        {
            SpecialType.System_SByte => "unchecked((uint)(byte)flag)",
            SpecialType.System_Byte => "unchecked((uint)(byte)flag)",
            SpecialType.System_Int16 => "unchecked((uint)(ushort)flag)",
            SpecialType.System_UInt16 => "unchecked((uint)(ushort)flag)",
            SpecialType.System_Int32 => "unchecked((uint)(int)flag)",
            SpecialType.System_UInt32 => "unchecked((uint)flag)",
            SpecialType.System_Int64 => "unchecked((ulong)(long)flag)",
            SpecialType.System_UInt64 => "unchecked((ulong)flag)",
            _ => null,
        };

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

