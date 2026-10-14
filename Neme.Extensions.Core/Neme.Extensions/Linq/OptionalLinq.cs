using Neme.Extensions.Contracts;

namespace Neme.Extensions.Linq;

public static class OptionalLinq
{
	public static Optional<TResult> Select<TSource, TResult>(
		this Optional<TSource> source,
		Func<TSource, TResult> selector)
	{
		Require.ArgumentNotNull(selector);

		return source.TryGetValue(out var value)
			? new(selector(value))
			: default;
	}

	public static Optional<TResult> SelectMany<TSource, TResult>(
		this Optional<TSource> source,
		Func<TSource, Optional<TResult>> selector)
	{
		Require.ArgumentNotNull(selector);

        return source.TryGetValue(out var value)
            ? selector(value)
			: default;
	}

	public static Optional<TResult> SelectMany<TSource, TCollection, TResult>(
		this Optional<TSource> source,
		Func<TSource, Optional<TCollection>> optionalSelector,
		Func<TSource, TCollection, TResult> resultSelector)
	{
		Require.ArgumentNotNull(optionalSelector);
		Require.ArgumentNotNull(resultSelector);

		return source.TryGetValue(out var value) && optionalSelector(value).TryGetValue(out var collectionValue)
			? new(resultSelector(value, collectionValue))
			: default;
	}

	public static Optional<TSource> Where<TSource>(this Optional<TSource> source, Func<TSource, bool> predicate)
	{
		Require.ArgumentNotNull(predicate);

		return source.TryGetValue(out var value) && predicate(value)
			? source
			: default;
	}

	public static int Count<TSource>(this Optional<TSource> source) =>
		source.HasValue ? 1 : 0;

	public static Optional<TResult> Cast<TSource, TResult>(this Optional<TSource> source) =>
		source.TryGetValue(out var value) ? new((TResult)(object)value!) : default;

	public static Optional<TResult> OfType<TSource, TResult>(this Optional<TSource> source) =>
		source.TryGetValue(out var value) && value is TResult result ? new(result) : default;

	public static bool Contains<TSource>(this Optional<TSource> source, TSource value, IEqualityComparer<TSource>? comparer = null)
	{
		comparer ??= EqualityComparer<TSource>.Default;
		return source.TryGetValue(out var sourceValue) && comparer.Equals(sourceValue, value);
	}

	public static bool Any<TSource>(this Optional<TSource> source) =>
		source.HasValue;

	public static bool Any<TSource>(this Optional<TSource> source, Func<TSource, bool> predicate)
	{
		Require.ArgumentNotNull(predicate);

		return source.TryGetValue(out var value) && predicate(value);
	}

	public static bool All<TSource>(this Optional<TSource> source, Func<TSource, bool> predicate)
	{
		Require.ArgumentNotNull(predicate);

		return !source.TryGetValue(out var value) || predicate(value);
	}

	public static Optional<TSource> Except<TSource>(
		this Optional<TSource> first,
		Optional<TSource> second,
		IEqualityComparer<TSource>? comparer = null)
	{
		comparer ??= EqualityComparer<TSource>.Default;

		return (first, second) switch
		{
			((true, var firstValue), (true, var secondValue)) =>
				comparer.Equals(firstValue!, secondValue!) ? default(Optional<TSource>) : firstValue!,
			_ => first,
		};
	}

	public static Optional<TSource> ExceptBy<TSource, TKey>(
		this Optional<TSource> first,
		Optional<TKey> second,
		Func<TSource, TKey> keySelector,
		IEqualityComparer<TKey>? comparer = null)
	{
		Require.ArgumentNotNull(keySelector);

		comparer ??= EqualityComparer<TKey>.Default;

		return (first, second) switch
		{
			((true, var firstValue), (true, var secondValue)) =>
				comparer.Equals(keySelector(firstValue!), secondValue!) ? default(Optional<TSource>) : firstValue!,
			_ => first,
		};
	}

	public static Optional<TSource> Intersect<TSource>(
		this Optional<TSource> first,
		Optional<TSource> second,
		IEqualityComparer<TSource>? comparer = null)
	{
		comparer ??= EqualityComparer<TSource>.Default;

		return (first, second) switch
		{
			((true, var firstValue), (true, var secondValue)) =>
				comparer.Equals(firstValue!, secondValue!) ? firstValue! : default(Optional<TSource>),
			_ => default,
		};
	}

	public static Optional<TSource> IntersectBy<TSource, TKey>(
		this Optional<TSource> first,
		Optional<TKey> second,
		Func<TSource, TKey> keySelector,
		IEqualityComparer<TKey>? comparer = null)
	{
		Require.ArgumentNotNull(keySelector);

		comparer ??= EqualityComparer<TKey>.Default;

		return (first, second) switch
		{
			((true, var firstValue), (true, var secondValue)) =>
				comparer.Equals(keySelector(firstValue!), secondValue!) ? firstValue! : default(Optional<TSource>),
			_ => default,
		};
	}

	public static Optional<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(
		this Optional<TFirst> first,
		Optional<TSecond> second)
	{
		return (first, second) switch
		{
			((true, var firstValue), (true, var secondValue)) => (firstValue!, secondValue!),
			_ => default,
		};
	}

	public static (Optional<TFirst> First, Optional<TSecond> Second) Unzip<TFirst, TSecond>(
		this Optional<(TFirst First, TSecond Second)> values)
    {
		return values switch
		{
			(true, var (first, second)) => (first, second),
			_ => default,
		};
	}

	public static IEnumerable<TSource> AsEnumerable<TSource>(this Optional<TSource> source)
	{
		return source.TryGetValue(out var value)
			? new TSource[] { value }
			: Enumerable.Empty<TSource>();
	}

	public static TSource[] ToArray<TSource>(this Optional<TSource> source)
	{
		return source.TryGetValue(out var value)
			? [value]
			: [];
	}

	public static List<TSource> ToList<TSource>(this Optional<TSource> source)
	{
		return source.TryGetValue(out var value)
			? [value]
			: [];
	}

	public static ImmutableArray<TSource> ToImmutableArray<TSource>(this Optional<TSource> source)
	{
		return source.TryGetValue(out var value)
			? [value]
			: [];
	}

	public static HashSet<TSource> ToHashSet<TSource>(this Optional<TSource> source)
	{
		return source.TryGetValue(out var value)
			? new HashSet<TSource> { value }
			: new HashSet<TSource>();
	}

	public static HashSet<TSource> ToHashSet<TSource>(this Optional<TSource> source, IEqualityComparer<TSource>? comparer)
	{
		return source.TryGetValue(out var value)
			? new HashSet<TSource>(comparer) { value }
			: new HashSet<TSource>(comparer);
	}
}
