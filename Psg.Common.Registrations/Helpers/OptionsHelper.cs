using Psg.Common.Registrations.Extensions;

namespace Psg.Common.Registrations.Helpers
{
    public static class OptionsHelper
    {
        public static Dictionary<string, object?> Get()
        {
            return [];
        }

        public static Dictionary<string, object?> Get(Dictionary<string, object?> options)
        {
            return options;
        }

        public static Dictionary<string, object?> Get(Dictionary<string, object?> defaultOptions, Dictionary<string, object?>? options)
        {
            options ??= [];
            return defaultOptions.ArrayMerge(options);
        }

        public static Dictionary<string, object?> Get(string key, object value)
        {
            return new() { { key, value } };
        }
    }
}
