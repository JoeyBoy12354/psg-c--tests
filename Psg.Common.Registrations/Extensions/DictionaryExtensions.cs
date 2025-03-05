using Newtonsoft.Json;

namespace Psg.Common.Registrations.Extensions
{
    public static class DictionaryExtensions
    {

        /// <summary>
        /// Merges 2 dictionaries. 
        /// <br/><br/>The <c>defaultOptions</c> will be overwritten by similar keys in the <c>providedOptions</c>.
        /// <br/><br/>Similar to the <c>array_merge</c> function in <c>PHP</c>.
        /// </summary>     
        /// <returns></returns>
        public static Dictionary<string, T?> ArrayMerge<T>(this Dictionary<string, T?>? defaultOptions,
                                                              Dictionary<string, T?>? providedOptions)
        {
            defaultOptions ??= new();

            if (providedOptions == null)
                return defaultOptions;


            // Iterate over provided options and update default options
            foreach (var kvp in providedOptions)
            {
                defaultOptions[kvp.Key] = kvp.Value;
            }

            return defaultOptions;
        }


        /// <summary>
        ///  If the value is a boolean, returns the boolean value.
        /// </summary>     
        public static bool TryGetValue<T>(this Dictionary<string, T?> options, string key)
        {
            if (!options.TryGetValue(key, out var value))
                return false;

            if (value == null)
                return false;

            if (value is bool b)
            {
                return b;
            }

            // something was found, so just return it as true
            return true;
        }


        public static bool TryGetValue<T>(this Dictionary<string, object?> options, string key, out T? result)
        {
            result = default;

            var value = options.GetValue<T>(key);

            if (value == null)
                return false;

            if (value.GetType() == typeof(T))
            {
                result = value;
                return true;
            }

            return false;
        }

        public static bool TryGetBool(this Dictionary<string, object?> options, string key)
        {
            var value = options.GetValue<bool?>(key);

            value ??= false;

            return value.Value;
        }

        public static object? GetValue(this Dictionary<string, object?> options, string key, object? defaultValue = null)
        {
            if (options == null || key == null)
                return defaultValue;

            if (!options.TryGetValue(key, out var value))
                return defaultValue;

            return value;
        }

        public static T? GetValue<T>(this Dictionary<string, object?> options, string key)
        {
            if (options == null || key == null)
                return default;

            if (!options.TryGetValue(key, out var value))
                return default;

            if (value == null)
                return default;

            if (value is T t)
            {
                return t;
            }

            // try deserialising to json and back
            // needed for int[] object from api request

            try
            {
                var jsonString = JsonConvert.SerializeObject(value);
                var obj = JsonConvert.DeserializeObject<T>(jsonString);

                return obj;
            }
            catch
            {
                return default;
            }


        }



        public static Dictionary<string, object?>? GetOptions(this Dictionary<string, object?> options, string key)
        {
            if (!options.TryGetValue(key, out var value))
                return null;

            if (value is Dictionary<string, object?> dic)
            {
                return dic;
            }

            return null;
        }

        public static void ArrayReplaceRecursive(this Dictionary<string, object?> target, params Dictionary<string, object?>[] dictionaries)
        {
            foreach (var dictionary in dictionaries)
            {
                MergeDictionaries(target, dictionary);
            }
        }




        private static void MergeDictionaries(Dictionary<string, object?> target, Dictionary<string, object?> source)
        {
            foreach (var kvp in source)
            {
                if (!target.ContainsKey(kvp.Key))
                {
                    target[kvp.Key] = kvp.Value;
                }
                else
                {
                    if (kvp.Value is Dictionary<string, object?> nestedSource && target[kvp.Key] is Dictionary<string, object?> nestedTarget)
                    {
                        // If both values are dictionaries, merge them recursively
                        MergeDictionaries(nestedTarget, nestedSource);
                    }
                    else
                    {
                        // Otherwise, just overwrite the target value
                        target[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
    }
}
