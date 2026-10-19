using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumToStringGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly string _enumTypeName = generationContext.EnumTypeName;
    private readonly ITypeSymbol? _enumUnderlyingType = generationContext.EnumType.EnumUnderlyingType;
    private readonly string? _underlyingTypeName = generationContext.UnderlyingTypeName;
    private readonly bool _isFlagsEnum = generationContext.IsFlagsEnum;
    private readonly ImmutableArray<IFieldSymbol> _enumMembers = generationContext.ConstantEnumMembers;

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

        textWriter.WriteLine("public string ToStringGeneratedInvariant() =>");

        using (textWriter.AddIndent())
            textWriter.WriteLine("ToStringGenerated(@enum, provider: global::System.Globalization.CultureInfo.InvariantCulture);");

        textWriter.WriteLine();
        textWriter.WriteLine("public string ToStringGenerated(global::System.IFormatProvider? provider = null)");
        textWriter.WriteLine("{");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("var enumName = @enum switch");
            textWriter.WriteLine("{");

            using (textWriter.AddIndent())
            {
                var seenValues = new HashSet<object>();

                foreach (var member in _enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var constantValue = member.ConstantValue;
                    if (constantValue is null || !seenValues.Add(constantValue))
                        continue;

                    textWriter.WriteLine($"{_enumTypeName}.{member.Name} => nameof({_enumTypeName}.{member.Name}),");
                }

                textWriter.WriteLine("_ => null,");
            }

            textWriter.WriteLine("};");
            textWriter.WriteLine();
            textWriter.WriteLine("if (enumName is not null)");

            using (textWriter.AddIndent())
                textWriter.WriteLine("return enumName;");

            if (_isFlagsEnum && _enumUnderlyingType is not null)
            {
                var unsignedTypeName = GeneratorUtilities.GetUnsignedEnumTypeName(_enumUnderlyingType);
                if (unsignedTypeName is null)
                    return false;
                
                var conversionExpression = GeneratorUtilities.GetUnsignedEnumValueExpression(_enumUnderlyingType, "@enum");
                if (conversionExpression is null)
                    return false;

                textWriter.WriteLine();
                textWriter.WriteLine($"{unsignedTypeName} remaining = {conversionExpression};");
                textWriter.WriteLine("if (remaining == 0)");

                using (textWriter.AddIndent())
                    textWriter.WriteLine($"return unchecked(({unsignedTypeName})0).ToString(provider);");

                textWriter.WriteLine();
                textWriter.WriteLine("var separator = provider is global::System.Globalization.CultureInfo cultureInfo");

                using (textWriter.AddIndent())
                {
                    textWriter.WriteLine("? $\"{cultureInfo.TextInfo.ListSeparator} \"");
                    textWriter.WriteLine(": \", \";");
                }

                textWriter.WriteLine();
                textWriter.WriteLine("var builder = new global::System.Text.StringBuilder();");
                textWriter.WriteLine();

                var flagConversionExpression = GeneratorUtilities.GetUnsignedEnumValueExpression(_enumUnderlyingType, "flag");
                if (flagConversionExpression is null)
                    return false;

                var emittedFlagValues = new HashSet<ulong>();

                foreach (var member in _enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (member.ConstantValue is not { } constantValue)
                        continue;

                    var memberValue = ConvertConstantToUInt64(constantValue);
                    if (memberValue == 0 || !emittedFlagValues.Add(memberValue))
                        continue;

                    textWriter.WriteLine($"AppendFlag(ref remaining, builder, separator, {_enumTypeName}.{member.Name}, nameof({_enumTypeName}.{member.Name}));");
                }

                textWriter.WriteLine();
                textWriter.WriteLine("if (remaining != 0)");
                textWriter.WriteLine("{");

                using (textWriter.AddIndent())
                {
                    textWriter.WriteLine("if (builder.Length > 0)");

                    using (textWriter.AddIndent())
                        textWriter.WriteLine("builder.Append(separator);");

                    textWriter.WriteLine();
                    textWriter.WriteLineNoTabs("#if NET6_0_OR_GREATER");
                    textWriter.WriteLine("builder.Append(provider, $\"{remaining}\");");
                    textWriter.WriteLineNoTabs("#else");
                    textWriter.WriteLine("builder.Append(remaining.ToString(provider));");
                    textWriter.WriteLineNoTabs("#endif");
                }

                textWriter.WriteLine("}");
                textWriter.WriteLine();
                textWriter.WriteLine("return builder.ToString();");
                textWriter.WriteLine();
                textWriter.WriteLine($"static void AppendFlag(ref {unsignedTypeName} remaining, global::System.Text.StringBuilder builder, string separator, {_enumTypeName} flag, string flagName)");
                textWriter.WriteLine("{");

                using (textWriter.AddIndent())
                {
                    textWriter.WriteLine($"{unsignedTypeName} flagValue = {flagConversionExpression};");
                    textWriter.WriteLine("if ((remaining & flagValue) != flagValue)");

                    using (textWriter.AddIndent())
                        textWriter.WriteLine("return;");

                    textWriter.WriteLine();
                    textWriter.WriteLine("if (builder.Length > 0)");

                    using (textWriter.AddIndent())
                        textWriter.WriteLine("builder.Append(separator);");

                    textWriter.WriteLine("builder.Append(flagName);");
                    textWriter.WriteLine("remaining &= ~flagValue;");
                }

                textWriter.WriteLine("}");
            }
            else
            {
                textWriter.WriteLine();
                textWriter.WriteLine("return @enum.ToString(provider);");
            }
        }

        textWriter.WriteLine("}");

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }

    private static ulong ConvertConstantToUInt64(object constantValue)
    {
        return constantValue switch
        {
            byte value => value,
            sbyte value => unchecked((ulong)value),
            short value => unchecked((ulong)value),
            ushort value => value,
            int value => unchecked((ulong)value),
            uint value => value,
            long value => unchecked((ulong)value),
            ulong value => value,
            _ => throw new InvalidOperationException($"Unsupported enum constant type '{constantValue.GetType().FullName}'."),
        };
    }

    }
