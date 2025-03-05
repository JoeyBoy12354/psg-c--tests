using System.Text.RegularExpressions;


namespace psg_automated_nunit_common.Managers
{
    public class PlaceholderReplacer
    {

        private readonly Dictionary<string, string> placeholderValues;

        public PlaceholderReplacer(Dictionary<string, string> values)
        {
            placeholderValues = values;
        }

        public string ReplacePlaceholders(string input)
        {
            string pattern = @"\{\{([^}]+)\}\}";

            return Regex.Replace(input, pattern, match =>
            {
                var placeholder = match.Groups[1]?.Value;

                if (string.IsNullOrEmpty(placeholder))
                    return input;

                if (placeholderValues.TryGetValue(placeholder, out var value))
                {
                    return value;
                }
                else
                {
                    // Placeholder not found, you can choose to handle this case differently.
                    // Here, we are returning the original placeholder.
                    return match.Value;
                }
            });
        }


    }
}


