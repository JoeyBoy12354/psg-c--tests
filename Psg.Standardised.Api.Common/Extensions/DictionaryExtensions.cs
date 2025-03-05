using System.Collections.ObjectModel;

namespace Psg.Standardised.Api.Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Extension method to convert an IReadOnlyDictionary<TKey, TValue> to normal Dictionary<TKey, TValue>
        /// that can be modified.
        /// </summary>      
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ConvertToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> readOnlyDict)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

            foreach (var kvp in readOnlyDict)
            {
                dictionary[kvp.Key] = kvp.Value;
            }

            return dictionary;
        }

        /// <summary>
        /// Extension method to create a read-only deep copy of a dictionary.
        /// </summary>      
        /// <returns></returns>
        public static ReadOnlyDictionary<TKey, TValue> CreateReadOnlyDeepCopy<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> original)
        {
            Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>();

            foreach (var kvp in original)
            {
                copy[kvp.Key] = kvp.Value;
            }

            return new ReadOnlyDictionary<TKey, TValue>(copy);
        }

        /// <summary>
        /// Extension method to create a deep copy of a dictionary, so as to not modify the original dictionary.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> DeepCopyDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> original)
        {
            Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>();

            foreach (var kvp in original)
            {
                copy[kvp.Key] = kvp.Value;
            }

            return copy;
        }
    }
}
