using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace psg_automated_nunit_shared.Helpers
{
    /// <summary>
    /// Reads in the <c>settings.json</c>
    /// </summary>
    public static class ConfigurationHelper
    {
        public static string Environment => GetEvironment();

        private const string _settingsFile = "testsettings";


        /// <summary>
        /// Loads the correct _settingsFile file, based on the environment.
        /// <br/> Example. if the Environment in the <c>settingsFile.json</c> is set to 'Development'
        /// <br/> the <c>settingsFile.Development.json</c> will be loaded
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetConfiguration()
        {   
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{_settingsFile}.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{_settingsFile}.{Environment}.json", optional: false)
                .AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        private static string GetEvironment()
        {
            // var currentDirectory = Directory.GetCurrentDirectory();

            // OVERRIDE IF DEBUGGING AND LOCAL FILE EXISTS!
            //if (Debugger.IsAttached)
            //{
            //    var localPath = Path.Combine(currentDirectory, $"{_settingsFile}.Local.json");

            //    if (File.Exists(localPath))
            //    {
            //        return "Local";
            //    }
            //}

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{_settingsFile}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = configurationBuilder.Build();

            var env = config.GetValue<string>("Environment") ?? "";

            return env;
        }

        /// <summary>
        /// Gets a section from the _settingsFile and deserialises into requested type.
        /// <br/> Use this method if section contains an array of objects. $ sign can be ommited.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonPath">Jsin path to the section</param>
        /// <returns></returns>
        public static T? GetSection<T>(string JsonPath) where T: class, new()
        {
            var json = GetSection(JsonPath);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var saveConfig = JsonConvert.DeserializeObject<T>(json);

            return saveConfig;
        }

        private static string GetSection(string jsonPath)
        {
            if (string.IsNullOrWhiteSpace(jsonPath))
                return "";

            jsonPath = "$." + jsonPath.TrimStart('$', '.');

            var environment = GetEvironment();
            var baseFolder = Directory.GetCurrentDirectory();

            var baseSettings = LoadJsonSettings(baseFolder, $"{_settingsFile}.json");
            var envSettings = string.IsNullOrWhiteSpace(environment)
                ? null
                : LoadJsonSettings(baseFolder, $"{_settingsFile}.{environment}.json");

            JObject mergedSettings = baseSettings;

            if (envSettings != null)
            {
                mergedSettings.Merge(envSettings, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            var jsonValue = mergedSettings.SelectToken(jsonPath)?.ToString();
            return string.IsNullOrWhiteSpace(jsonValue) ? "" : jsonValue;
        }

        private static JObject LoadJsonSettings(string baseFolder, string fileName)
        {
            var filePath = Path.Combine(baseFolder, fileName);

            if (!File.Exists(filePath))
                return new JObject();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<JObject>(json) ?? new JObject();
        }
       
    }
}
