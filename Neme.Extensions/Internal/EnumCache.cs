using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal;

internal sealed class EnumCache<TEnum>
    where TEnum : struct, Enum
{
    private ValueLazy<bool> _isFlagsLazy;
    private ValueLazy<Type> _underlyingTypeLazy;
    private ValueLazy<ImmutableArray<TEnum>> _valuesLazy;
    private ValueLazy<ImmutableArray<ulong>> _uint64ValuesLazy;
    private ValueLazy<ImmutableArray<long>> _int64ValuesLazy;

    private bool IsFlags
    {
        get => _isFlagsLazy.EnsureInitialized(() =>
            typeof(TEnum).GetCustomAttribute<FlagsAttribute>() is not null);
    }

    private Type UnderlyingType
    {
        get => _underlyingTypeLazy.EnsureInitialized(() =>
            typeof(TEnum).GetEnumUnderlyingType());
    }

    private ImmutableArray<TEnum> Values
    {
        get
        {
            return _valuesLazy.EnsureInitialized(() => ImmutableCollectionsMarshal.AsImmutableArray(
#if NET5_0_OR_GREATER
                Enum.GetValues<TEnum>()
#else
                (TEnum[])Enum.GetValues(typeof(TEnum))
#endif
                ));
        }
    }

    private ImmutableArray<ulong> UInt64Values
    {
        get
        {
            return _uint64ValuesLazy.EnsureInitialized(() =>
            {
                var builder = ImmutableArray.CreateBuilder<ulong>(Values.Length);

                foreach (var value in Values)
                    builder.Add(Convert.ToUInt64(value));

                return builder.MoveToImmutable();
            });
        }
    }

    private ImmutableArray<long> Int64Values
    {
        get
        {
            return _int64ValuesLazy.EnsureInitialized(() =>
            {
                var builder = ImmutableArray.CreateBuilder<long>(Values.Length);

                foreach (var value in Values)
                    builder.Add(Convert.ToInt64(value));

                return builder.MoveToImmutable();
            });
        }
    }

    public static EnumCache<TEnum> Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, () => new EnumCache<TEnum>())!;
    }

    public bool FlagsDefined(TEnum value)
    {
        if (!IsFlags)
            ThrowNotFlags(value);

        return UnderlyingType.IsUnsignedPrimitiveInteger
            ? FlagsDefinedUInt64(value)
            : FlagsDefinedInt64(value);

        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowNotFlags(TEnum value) =>
            throw new ArgumentException2(nameof(value), value, "The enum type must be a [Flags] enum.");
    }

    private bool FlagsDefinedUInt64(TEnum value)
    {
        ulong allFlags = default;

        foreach (var flag in UInt64Values)
            allFlags |= flag;

        return (Convert.ToUInt64(value) | allFlags) == allFlags;
    }

    private bool FlagsDefinedInt64(TEnum value)
    {
        long allFlags = default;

        foreach (var flag in Int64Values)
            allFlags |= flag;

        return (Convert.ToInt64(value) | allFlags) == allFlags;
    }
}
