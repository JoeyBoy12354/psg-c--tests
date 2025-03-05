using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace psg_automated_nunit_common.Helpers
{
    /// <summary>
    /// Reads in the <c>appsettings.json</c>
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Gets a section from the appsettings and deserialises into requested type.
        /// <br/> Use this method if section contains an array of objects. $ sign can be ommited.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonPath">Jsin path to the section</param>
        /// <returns></returns>
        public static T? GetSection<T>(string JsonPath) where T: class
        {
            var json = GetSection(JsonPath);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var saveConfig = JsonConvert.DeserializeObject<T>(json);

            return saveConfig;
        }

        private static string GetEvironment()
        {
           return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
        }


        private static string GetSection(string jsonPath)
        {
            if (string.IsNullOrWhiteSpace(jsonPath))
                return "";

            // fix dollar sign
            jsonPath = jsonPath.Replace("$.", "");
            jsonPath = "$." + jsonPath;

            var environment = GetEvironment();

            var baseFolder = Directory.GetCurrentDirectory();

            var appsettings = "";

            if (!string.IsNullOrWhiteSpace(environment))
            {
                var f_env = $"appsettings.{environment}.json";
                var json_env = ReadSaveConfiguration(baseFolder, f_env);

                if (!string.IsNullOrWhiteSpace(json_env))
                    appsettings = json_env;
            }

            if(string.IsNullOrWhiteSpace(appsettings))
            {
                var f = $"appsettings.json";
                appsettings = ReadSaveConfiguration(baseFolder, f);

                if (string.IsNullOrWhiteSpace(appsettings))
                    return "";
            }         

            var jsonObj = JsonConvert.DeserializeObject<JObject>(appsettings);

            string? json = jsonObj?.SelectToken(jsonPath)?.ToString();

            if (string.IsNullOrWhiteSpace(json))
                return "";

            return json;
        }


        private static string ReadSaveConfiguration(string baseFolder, string appSettings)
        {
            var filePath = Path.Combine(baseFolder, appSettings);

            var json = ReadSaveConfiguration(filePath);

            return json;
        }

        private static string ReadSaveConfiguration(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return json;
        }

        /// <summary>
        /// Adds multiple configurations from an array of objects in the appsettings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="objectArrayFieldName">The name of the field which contains the array on configurations</param>
        public static void AddConfigurations<T>(IServiceCollection serviceCollection, string objectArrayFieldName) where T : class
        {
            if (string.IsNullOrEmpty(objectArrayFieldName))
                return;

            var namespaceType = typeof(T);

            string namespaceName = namespaceType.Namespace!;

            var config = GetSection<T>(namespaceType.Name);

            if (config == null)
                return;

            foreach (var prop in config.GetType().GetProperties())
            {
                if (prop.Name == objectArrayFieldName)
                {
                    var value = prop.GetValue(config);

                    if (value is IEnumerable<object> configs)
                    {
                        AddConfigsFromObjects(serviceCollection,
                               namespaceName,
                                                 configs);

                        return;
                    }
                }
            }
        }


        private static void AddConfigsFromObjects(IServiceCollection serviceCollection,
                                                string configfolderNamespace,
                                                IEnumerable<object> configs)
        {
            foreach (var config in configs)
            {
                if(config is JObject c)
                {
                    AddConfigsFromJObject(serviceCollection, configfolderNamespace, c);
                }
            }
        }

        private static void AddConfigsFromJObject(IServiceCollection serviceCollection, 
                                                  string configfolderNamespace, 
                                                  JObject c)
        { 
            foreach (var prop in c)
            {
                string key = prop.Key;
                JToken? value = prop.Value;

                if (value == null)
                    continue;

                // Get the corresponding Type based on the key
                // ex. psg_automated_nunit_shared.Configurations.FileWriterConfiguration
                var type = Type.GetType($"{configfolderNamespace}.{key}");               

                // If the type exists, create an instance and populate it
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);

                    if (instance != null)
                    {
                        JsonConvert.PopulateObject(value.ToString(), instance);
                    }

                    // Add instance to IServiceCollection
                    serviceCollection.AddSingleton(type, instance ?? new());
                }
            }
        }
    }
}
