using Neme.Extensions.Contracts;

namespace Neme.Extensions.Collections;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary)
    {
        public Optional<TValue> GetValueOrNone(TKey key)
        {
            Require.ArgumentNotNull(dictionary);

            return dictionary.TryGetValue(key, out var value) ? value : Optional<TValue>.None;
        }
    }
}
