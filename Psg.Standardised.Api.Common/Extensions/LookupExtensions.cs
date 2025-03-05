using Psg.Standardised.Api.Common.Models;

namespace Psg.Standardised.Api.Common.Extensions
{
    public static class LookupExtensions
    {
        /// <summary>
        /// Filters lookup dictionary by key
        /// </summary>
        /// <returns>KeyValue pair from dictionary or default if index out of bounds </returns>
        public static IReadOnlyDictionary<int, string> FilterLookup(this IReadOnlyDictionary<int, string> dic, int index)
        {
            IReadOnlyDictionary<int, string> result = dic.Where(w => w.Key == index)
                .ToDictionary(k => k.Key, v => v.Value);

            if (result.Any())
                return result;
            else
                return default;
        }

        /// <summary>
        /// Filters lookup dictionary by key
        /// </summary>
        /// <returns>KeyValue pair from dictionary or default if index out of bounds </returns>
        public static IReadOnlyDictionary<int, string[]> FilterLookup(this IReadOnlyDictionary<int, string[]> dic, int index)
        {
            IReadOnlyDictionary<int, string[]> result = dic.Where(w => w.Key == index)
                .ToDictionary(k => k.Key, v => v.Value);

            if (result.Any())
                return result;
            else
                return default;
        }

        /// <summary>
        /// Filters lookup dictionary by key
        /// </summary>
        /// <returns>KeyValue pair from dictionary or default if index out of bounds </returns>
        public static IReadOnlyDictionary<string, int> FilterLookup(this IReadOnlyDictionary<string, int> dic, string index)
        {
            IReadOnlyDictionary<string, int> result = dic.Where(w => w.Key == index)
                .ToDictionary(k => k.Key, v => v.Value);

            if (result.Any())
                return result;
            else
                return default;
        }

        /// <summary>
        /// Filters lookup dictionary by key
        /// </summary>
        /// <returns>KeyValue pair from dictionary or default if index out of bounds </returns>
        public static IReadOnlyDictionary<string, int[]> FilterLookup(this IReadOnlyDictionary<string, int[]> dic, string index)
        {
            IReadOnlyDictionary<string, int[]> result = dic.Where(w => w.Key == index)
                .ToDictionary(k => k.Key, v => v.Value);

            if (result.Any())
                return result;
            else
                return default;
        }
    }
}