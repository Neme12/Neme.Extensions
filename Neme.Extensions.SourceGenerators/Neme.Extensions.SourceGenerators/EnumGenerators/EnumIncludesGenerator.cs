using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumIncludesGenerator
{
    public static string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumUnderlyingType = enumType.EnumUnderlyingType;
        if (enumUnderlyingType is null)
            return null;

        var typeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var underlyingTypeName = enumUnderlyingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

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
            return null;

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public bool Includes({typeName} flag)");
            textWriter.WriteLine('{');
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"if (!global::System.Numerics.BitOperations.IsPow2({isPow2Operand}))");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine("ThrowNotOneFlag(flag);");
            textWriter.WriteLine();
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("return (value & flag) != 0;");
            textWriter.WriteLine();
            textWriter.WriteLine("[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]");
            textWriter.WriteLine($"static void ThrowNotOneFlag({typeName} flag) =>");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine($"throw new global::System.ArgumentException($\"The value '{{flag}}' is not a single flag value for enum type '{{nameof({enumType})}}'.\", nameof(flag));");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine('}');

            textWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return textWriter.InnerWriter.ToString();
        }
    }
}

