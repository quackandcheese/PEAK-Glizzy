using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Glizzy.Extensions
{
    internal static class DictionaryExtensions
    {
        public static bool TryGetKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value, out TKey? key)
        {
            foreach (var kvp in dictionary)
            {
                if (Equals(kvp.Value, value))
                {
                    key = kvp.Key!;
                    return true;
                }
            }

            key = default;
            return false;
        }


        public static TKey? GetKeyOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
        {
            if (dictionary.TryGetKey(value, out TKey? key))
            {
                return key;
            }

            return default;
        }

        public static TKey? GetKeyOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value, TKey defaultKey)
        {
            if (dictionary.TryGetKey(value, out TKey? key))
            {
                return key;
            }

            return defaultKey;
        }

        public static bool TryGetValue<T>(this Dictionary<string, T> dictionary, string key, out T? value, bool ignoreKeyCase)
        {
            if (!ignoreKeyCase)
            {
                return dictionary.TryGetValue(key, out value);
            }

            foreach (var kvp in dictionary)
            {
                if (string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = kvp.Value!;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
