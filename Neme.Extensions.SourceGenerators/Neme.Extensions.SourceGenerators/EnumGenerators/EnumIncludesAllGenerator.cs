using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumIncludesAllGenerator
{
    public static string GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public bool IncludesAll({enumTypeName} flag) =>");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("(@enum & flag) == flag;");
            textWriter.WriteLine();

            cancellationToken.ThrowIfCancellationRequested();

            textWriter.Indent = baselineIndent;
            textWriter.WriteLine($"public bool IncludesAll(params global::System.ReadOnlySpan<{enumTypeName}> flags)");
            textWriter.WriteLine('{');
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("foreach (var flag in flags)");
            textWriter.WriteLine('{');
            textWriter.Indent = baselineIndent + 2;
            textWriter.WriteLine("if (!IncludesAll(@enum, flag))");
            textWriter.Indent = baselineIndent + 3;
            textWriter.WriteLine("return false;");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine('}');
            textWriter.WriteLine();
            textWriter.WriteLine("return true;");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine('}');

            textWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return textWriter.InnerWriter.ToString();
        }
    }
}

