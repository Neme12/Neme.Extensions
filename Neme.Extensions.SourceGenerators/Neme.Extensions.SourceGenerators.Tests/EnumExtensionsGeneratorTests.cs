using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neme.Extensions.SourceGenerators.EnumGenerators;
using System.CodeDom.Compiler;
using System.Text;

namespace Neme.Extensions.SourceGenerators.Tests;

public sealed class EnumExtensionsGeneratorTests
{
    public sealed class NonFlagsEnum
    {
        [Fact]
        public void ToStringGenerated_MatchesExpectedOutput()
        {
            var method = EmitToStringGenerated("""
                namespace Demo;

                public enum TrafficLight
                {
                    Red = 0,
                    Amber = 1,
                    Green = 2,
                }
                """, "Demo.TrafficLight");

            var expected = @"public string ToStringGenerated(global::System.IFormatProvider? provider = null)
{
    var enumName = @enum switch
    {
        global::Demo.TrafficLight.Red => nameof(global::Demo.TrafficLight.Red),
        global::Demo.TrafficLight.Amber => nameof(global::Demo.TrafficLight.Amber),
        global::Demo.TrafficLight.Green => nameof(global::Demo.TrafficLight.Green),
        _ => null,
    };

    if (enumName is not null)
        return enumName;

    return @enum.ToString(provider);
}";

            Assert.Equal(NormalizeLineEndings(expected), NormalizeLineEndings(method));
        }
    }

    public sealed class FlagsEnum
    {
        [Fact]
        public void ToStringGenerated_MatchesExpectedOutput()
        {
            var method = EmitToStringGenerated("""
                namespace Demo;

                [System.Flags]
                public enum FileAccess
                {
                    None = 0,
                    Read = 1,
                    Write = 2,
                    ReadWrite = Read | Write,
                    Execute = 4,
                }
                """, "Demo.FileAccess");

            var expected = @"public string ToStringGenerated(global::System.IFormatProvider? provider = null)
{
    var enumName = @enum switch
    {
        global::Demo.FileAccess.None => nameof(global::Demo.FileAccess.None),
        global::Demo.FileAccess.Read => nameof(global::Demo.FileAccess.Read),
        global::Demo.FileAccess.Write => nameof(global::Demo.FileAccess.Write),
        global::Demo.FileAccess.ReadWrite => nameof(global::Demo.FileAccess.ReadWrite),
        global::Demo.FileAccess.Execute => nameof(global::Demo.FileAccess.Execute),
        _ => null,
    };

    if (enumName is not null)
        return enumName;

    uint remaining = unchecked((uint)(int)@enum);
    if (remaining == 0)
        return unchecked((uint)0).ToString(provider);

    var separator = provider is global::System.Globalization.CultureInfo cultureInfo
        ? $""{cultureInfo.TextInfo.ListSeparator} ""
        : "", "";

    var builder = new global::System.Text.StringBuilder();

    AppendFlag(ref remaining, builder, separator, global::Demo.FileAccess.Read, nameof(global::Demo.FileAccess.Read));
    AppendFlag(ref remaining, builder, separator, global::Demo.FileAccess.Write, nameof(global::Demo.FileAccess.Write));
    AppendFlag(ref remaining, builder, separator, global::Demo.FileAccess.ReadWrite, nameof(global::Demo.FileAccess.ReadWrite));
    AppendFlag(ref remaining, builder, separator, global::Demo.FileAccess.Execute, nameof(global::Demo.FileAccess.Execute));

    if (remaining != 0)
    {
        if (builder.Length > 0)
            builder.Append(separator);

#if NET6_0_OR_GREATER
        builder.Append(provider, $""{remaining}"");
#else
        builder.Append(remaining.ToString(provider));
#endif
    }

    return builder.ToString();

    static void AppendFlag(ref uint remaining, global::System.Text.StringBuilder builder, string separator, global::Demo.FileAccess flag, string flagName)
    {
        uint flagValue = unchecked((uint)(int)flag);
        if ((remaining & flagValue) != flagValue)
            return;

        if (builder.Length > 0)
            builder.Append(separator);
        builder.Append(flagName);
        remaining &= ~flagValue;
    }
}";

            Assert.Equal(NormalizeLineEndings(expected), NormalizeLineEndings(method));
        }
    }

    private static string EmitToStringGenerated(string source, string enumMetadataName)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile("Neme.Polyfills.dll"));

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratedEnumTests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var enumType = compilation.GetTypeByMetadataName(enumMetadataName);
        Assert.NotNull(enumType);

        var context = TestUtilities.CreateEnumGenerationContext(compilation, enumType);

        var generator = new EnumToStringGenerator(context);

        using var writer = new IndentedTextWriter(new StringWriter(), "    ") { Indent = 0 };

        var emitted = generator.EmitExtensionSource(default, writer, CancellationToken.None);
        Assert.True(emitted);

        var generatedOutput = ((StringWriter)writer.InnerWriter).ToString();
        var generatedSyntaxRoot = CSharpSyntaxTree.ParseText(generatedOutput, parseOptions).GetRoot();
        var generatedToStringMethod = generatedSyntaxRoot.ChildNodes().OfType<GlobalStatementSyntax>()
            .First(m => m.Statement is LocalFunctionStatementSyntax localFunction && localFunction.Identifier.Text == "ToStringGenerated");

        return generatedToStringMethod.ToString();
    }

    private static string NormalizeLineEndings(string value)
    {
        var normalized = value.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();
        var lines = normalized.Split('\n');
        var builder = new StringBuilder(normalized.Length);

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimEnd();
            builder.Append(trimmed);

            if (i < lines.Length - 1)
                builder.Append('\n');
        }

        return builder.ToString();
    }
}
