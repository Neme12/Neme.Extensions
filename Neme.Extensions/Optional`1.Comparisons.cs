using Neme.Extensions.Conversions;
using System.Collections;
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

    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
    {
        if (comparer is null)
            throw new ArgumentNullException(nameof(comparer));

        return other is Optional<T> optional && Equals(optional, comparer.AsGeneric<T>());
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Optional<T> optional && Equals(optional);

    public int CompareTo(Optional<T> other) =>
        CompareTo(other, Comparer<T>.Default);

    public int CompareTo(Optional<T> other, IComparer<T> elementComparer)
    {
        if (elementComparer is null)
            throw new ArgumentNullException(nameof(elementComparer));

        return (_hasValue, other._hasValue) switch
        {
            (true, true) => elementComparer.Compare(_value!, other._value!),
            (true, false) => 1,
            (false, true) => -1,
            (false, false) => 0,
        };
    }

    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        if (comparer is null)
            throw new ArgumentNullException(nameof(comparer));

        return other switch
        {
            null => 1,
            Optional<T> optional => CompareTo(optional, comparer.AsGeneric<T>()),
            _ => ThrowHelper.ThrowArgument_ObjectMustBeOfType(nameof(other), typeof(Optional<T>)),
        };
    }

    int IComparable.CompareTo(object? obj)
    {
        return obj switch
        {
            null => 1,
            Optional<T> optional => CompareTo(optional),
            _ => ThrowHelper.ThrowArgument_ObjectMustBeOfType(nameof(obj), typeof(Optional<T>)),
        };
    }

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

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        if (comparer is null)
            throw new ArgumentNullException(nameof(comparer));

        return GetHashCode(comparer.AsGeneric<T>());
    }

    private static Func<T, T, bool>? s_opEqualityMethod;
    private static bool s_opEqualityMethodInitialized;
    private static object? s_opEqualityMethodLock;

    private static Func<T, T, bool>? s_opInequalityMethod;
    private static bool s_opInequalityMethodInitialized;
    private static object? s_opInequalityMethodLock;

    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
        return (left._hasValue, right._hasValue) switch
        {
            (true, true) => OperatorEquals(left._value!, right._value!),
            (false, false) => true,
            _ => false,
        };
    }

    public static bool operator !=(Optional<T> left, Optional<T> right)
    {
        return (left._hasValue, right._hasValue) switch
        {
            (true, true) => OperatorNotEquals(left._value!, right._value!),
            (false, false) => false,
            _ => true,
        };
    }

    private static bool OperatorEquals(T left, T right)
    {
        var method = LazyInitializer.EnsureInitialized(
            ref s_opEqualityMethod,
            ref s_opEqualityMethodInitialized,
            ref s_opEqualityMethodLock,
            static () => GetEqualityOperatorMethod("op_Equality"));

        Debug.Assert(s_opEqualityMethodInitialized);

        if (method is not null)
            return method.Invoke(left, right);

        return EqualityComparer<T>.Default.Equals(left, right);
    }

    private static bool OperatorNotEquals(T left, T right)
    {
        var method = LazyInitializer.EnsureInitialized(
            ref s_opInequalityMethod,
            ref s_opInequalityMethodInitialized,
            ref s_opInequalityMethodLock,
            static () => GetEqualityOperatorMethod("op_Inequality"));

        Debug.Assert(s_opInequalityMethodInitialized);

        if (method is not null)
            return method.Invoke(left, right);

        return !EqualityComparer<T>.Default.Equals(left, right);
    }

    private static Func<T, T, bool>? GetEqualityOperatorMethod(string methodName)
    {
        Debug.Assert(methodName is "op_Equality" or "op_Inequality");

        var method = typeof(T).GetMethod(
            methodName,
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
    }
}
