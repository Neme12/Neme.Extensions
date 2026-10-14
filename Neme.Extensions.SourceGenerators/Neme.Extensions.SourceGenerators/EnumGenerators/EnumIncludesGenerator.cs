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

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public bool Includes({typeName} flag)");
            textWriter.WriteLine('{');
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine($"if (!global::System.Numerics.BitOperations.IsPow2(({underlyingTypeName})flag))");
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine($"ThrowNotOneFlag(flag);");
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

