using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumValueNamesGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public string? GetTypeMemberSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var nameData = GetNameData(cancellationToken);

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            {
                textWriter.WriteLine("private static string[] ValueOrderedNames");
                textWriter.WriteLine("{");

                using (textWriter.WithIndent(baselineIndent + 1))
                {
                    textWriter.WriteLine("get => global::System.Threading.LazyInitializer.EnsureInitialized(ref field, static () =>");
                    textWriter.WriteLine("[");

                    using (textWriter.WithIndent(baselineIndent + 2))
                    {
                        foreach (var name in nameData.AllNames)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            textWriter.WriteLine($"nameof({enumTypeName}.{name}),");
                        }
                    }

                    textWriter.WriteLine("]!);");
                }

                textWriter.WriteLine("}");
            }

            return textWriter.InnerWriter.ToString();
        }
    }

    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var nameData = GetNameData(cancellationToken);

        using (var switchWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            {
                switchWriter.WriteLine("public global::System.ReadOnlySpan<string> ValueNames =>");

                using (switchWriter.WithIndent(baselineIndent + 1))
                {
                    switchWriter.WriteLine("@enum switch");
                    switchWriter.WriteLine("{");

                    using (switchWriter.WithIndent(baselineIndent + 2))
                    {
                        foreach (var group in nameData.Groups)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            switchWriter.WriteLine($"{enumTypeName}.{group.MemberName} => ValueOrderedNames.AsSpan({group.Start}, {group.Length}),");
                        }

                        switchWriter.WriteLine("_ => [],");
                    }

                    switchWriter.WriteLine("};");
                }
            }

            switchWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return switchWriter.InnerWriter.ToString();
        }
    }

    private NameData GetNameData(CancellationToken cancellationToken)
    {
        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        if (enumMembers.Length == 0)
            return new NameData([], []);

        var orderedMembers = new OrderedMember[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var member = enumMembers[i];
            orderedMembers[i] = new OrderedMember(member.Name, member.ConstantValue!, i);
        }

        Array.Sort(orderedMembers, static (left, right) =>
        {
            var valueComparison = ((IComparable)left.ConstantValue).CompareTo(right.ConstantValue);
            if (valueComparison != 0)
                return valueComparison;

            return left.DeclarationIndex.CompareTo(right.DeclarationIndex);
        });

        var allNames = new string[orderedMembers.Length];
        for (var i = 0; i < orderedMembers.Length; ++i)
            allNames[i] = orderedMembers[i].MemberName;

        var groupCount = 1;
        for (var i = 1; i < orderedMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (((IComparable)orderedMembers[i - 1].ConstantValue).CompareTo(orderedMembers[i].ConstantValue) != 0)
                ++groupCount;
        }

        var groups = new NameRange[groupCount];
        var groupIndex = 0;
        var groupStart = 0;

        for (var i = 1; i <= orderedMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var isGroupBoundary = i == orderedMembers.Length ||
                ((IComparable)orderedMembers[i - 1].ConstantValue).CompareTo(orderedMembers[i].ConstantValue) != 0;

            if (!isGroupBoundary)
                continue;

            groups[groupIndex++] = new NameRange(
                orderedMembers[groupStart].MemberName,
                groupStart,
                i - groupStart);
            groupStart = i;
        }

        return new NameData(allNames, groups);
    }

    private readonly record struct NameData(string[] AllNames, NameRange[] Groups);

    private readonly record struct NameRange(string MemberName, int Start, int Length);

    private readonly record struct OrderedMember(string MemberName, object ConstantValue, int DeclarationIndex);
}
