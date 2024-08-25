using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Neme.Extensions;

public readonly partial struct Optional<T>
{
	public bool Equals(Optional<T> other) =>
        Equals(other, EqualityComparer<T>.Default);

    public bool Equals(Optional<T> other, IEqualityComparer<T> elementComparer)
	{
        if (elementComparer is null)
            throw new ArgumentNullException(nameof(elementComparer));

        return (_hasValue, other._hasValue) switch
        {
            (true, true) => elementComparer.Equals(_value!, other._value!),
            (false, false) => true,
            _ => false,
        };
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Optional<T> other && Equals(other);

    public override int GetHashCode() =>
        GetHashCode(EqualityComparer<T>.Default);

    public int GetHashCode(IEqualityComparer<T> elementComparer)
    {
        if (elementComparer is null)
            throw new ArgumentNullException(nameof(elementComparer));

        if (_hasValue)
            return elementComparer.GetHashCode(_value!);

        return -1;
    }

    private static Func<T, T, bool>? s_opEqualityMethod;
    private static bool s_opEqualityMethodInitialized;
    private static object? s_opEqualityMethodLock;

    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
        return (left._hasValue, right._hasValue) switch
        {
            (true, true) => OperatorEquals(left._value!, right._value!),
            (false, false) => true,
            _ => false,
        };
    }

    public static bool operator !=(Optional<T> left, Optional<T> right) =>
        !(left == right);

    private static bool OperatorEquals(T left, T right)
    {
        var method = LazyInitializer.EnsureInitialized(
            ref s_opEqualityMethod,
            ref s_opEqualityMethodInitialized,
            ref s_opEqualityMethodLock,
            static () =>
            {
                var method = typeof(T).GetMethod(
                    "op_Equality",
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    genericParameterCount: 0,
#endif
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly,
                    binder: null,
                    [typeof(T), typeof(T)],
                    modifiers: null);

                if (method is null)
                    return null;

#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
                if (method.ContainsGenericParameters)
                    return null;
#endif

                if (method.ReturnType != typeof(bool))
                    return null;

#if NET5_0_OR_GREATER
                return method.CreateDelegate<Func<T, T, bool>>();
#else
                return (Func<T, T, bool>)method.CreateDelegate(typeof(Func<T, T, bool>));
#endif
            });

        Debug.Assert(s_opEqualityMethodInitialized);

        if (method is not null)
            return method.Invoke(left, right);

        return EqualityComparer<T>.Default.Equals(left, right);
    }
}
