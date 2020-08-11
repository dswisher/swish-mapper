
using System;
using System.Collections.Generic;

namespace SwishMapper.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue FindOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> factory)
        {
            // If an item exists, return it.
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            // Not yet there, create it and add it.
            TValue val = factory();

            dict.Add(key, val);

            return val;
        }
    }
}
