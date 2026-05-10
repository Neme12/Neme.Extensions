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

    public bool Equals(Optional<T> other, IEqualityComparer<T>? valueComparer)
	{
        valueComparer ??= EqualityComparer<T>.Default;

        return (_hasValue, other._hasValue) switch
        {
            (true, true) => valueComparer.Equals(_value!, other._value!),
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

    public int CompareTo(Optional<T> other, IComparer<T>? valueComparer)
    {
        valueComparer ??= Comparer<T>.Default;

        return (_hasValue, other._hasValue) switch
        {
            (true, true) => valueComparer.Compare(_value!, other._value!),
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

    public int GetHashCode(IEqualityComparer<T>? valueComparer)
    {
        valueComparer ??= EqualityComparer<T>.Default;

        if (_hasValue)
            return valueComparer.GetHashCode(_value!);

        return -1;
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        if (comparer is null)
            throw new ArgumentNullException(nameof(comparer));

        return GetHashCode(comparer.AsGeneric<T>());
    }

#pragma warning disable RS0042
    private static ValueLazy<Func<T, T, bool>?> s_opEqualityMethodLazy;
    private static ValueLazy<Func<T, T, bool>?> s_opInequalityMethodLazy;
#pragma warning restore RS0042

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
        var method = s_opEqualityMethodLazy.EnsureInitialized(static () => GetEqualityOperatorMethod("op_Equality"));
        if (method is not null)
            return method.Invoke(left, right);

        return EqualityComparer<T>.Default.Equals(left, right);
    }

    private static bool OperatorNotEquals(T left, T right)
    {
        var method = s_opInequalityMethodLazy.EnsureInitialized(static () => GetEqualityOperatorMethod("op_Inequality"));
        if (method is not null)
            return method.Invoke(left, right);

        return !EqualityComparer<T>.Default.Equals(left, right);
    }

    private static Func<T, T, bool>? GetEqualityOperatorMethod(string methodName)
    {
        Debug.Assert(methodName is "op_Equality" or "op_Inequality");

        return typeof(T).GetMethodDelegate<Func<T, T, bool>>(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
    }
}
