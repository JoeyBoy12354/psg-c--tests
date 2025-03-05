using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Refit;
using psg_automated_nunit_shared.Services;
using psg_automated_nunit_shared.Writers;
using Refit;
using Serilog;

namespace psg_automated_nunit_shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the TestConfiguration and any writer configurations from the appsettings.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        public static void AddTestConfiguration(this ServiceCollection serviceCollection, 
                                      IConfigurationRoot configuration)
        {
            TestConfiguration config = new();
            configuration.GetSection("TestConfiguration").Bind(config);
            serviceCollection.AddSingleton(config);

            var json = JsonConvert.SerializeObject(config);
            Log.Logger.Information("TestConfiguration: {config}",json);


            serviceCollection.AddWriters(config);

            AddRefitClients(serviceCollection, config);

            serviceCollection.AddScoped<IOtpService, OtpService>();

        }

        public static void AddFicaApiConfiguration(this ServiceCollection serviceCollection,
                                                   IConfigurationRoot configuration)
        {
            FicaApiConfiguration config = new();
            configuration.GetSection("FicaApiConfiguration").Bind(config);
            serviceCollection.AddSingleton(config);

            var json = JsonConvert.SerializeObject(config);
            Log.Logger.Information("FicaApiConfiguration: {config}", json);

            serviceCollection.AddRefitClient<IAzureExternalTokenProviderApi>()
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(config.Url));
        }
    



        private static void AddWriters(this ServiceCollection serviceCollection, 
                                       TestConfiguration config)
        {
            Type namespaceType = typeof(TestConfiguration);
            string namespaceName = namespaceType.Namespace!;

            // Manually read in the SaveConfiguration section from the appsettings,
            // to deserialise the WriteTo section properly into a list of objects
            // then cast those objects into their respective types
            // and add them to the ServiceCollection

            var saveConfig = ConfigurationHelper.GetSection<SaveConfiguration>("TestConfiguration.SaveConfiguration"); 

            if (saveConfig == null)
                return;           

            config.SaveConfiguration = saveConfig;

            if (config.SaveConfiguration?.WriteTo == null)
                return;

            foreach (JObject c in config.SaveConfiguration.WriteTo)
            {

                foreach (var prop in c)
                {
                    string key = prop.Key;
                    JToken? value = prop.Value;

                    if (value == null)
                        continue;

                    // Get the corresponding Type based on the key
                    // ex. psg_automated_nunit_shared.Configurations.FileWriterConfiguration
                    Type? type = Type.GetType($"{namespaceName}.{key}");

                    // If the type exists, create an instance and populate it
                    if (type != null)
                    {
                        object? instance = Activator.CreateInstance(type);

                        if (instance != null)
                        {
                            JsonConvert.PopulateObject(value.ToString(), instance);

                            if(instance is ApiWriterConfiguration apiConfig)
                            {
                                AddRefitWriters(serviceCollection, apiConfig);
                            }
                        }

                        // Add instance to IServiceCollection
                        serviceCollection.AddSingleton(type, instance ?? new());
                    }

                }
            }

            serviceCollection.RegisterWriters();
        }

        private static void RegisterWriters(this ServiceCollection services)
        {
            var assembly = typeof(TestResultApiWriter).Assembly;        

            // Find all types that implement ITestResultWriter
            var writerTypes = assembly.GetTypes()
                .Where(t => typeof(ITestResultWriter).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            // Register each implementation
            foreach (var type in writerTypes)
            {
                services.AddScoped(typeof(ITestResultWriter), type);
            }

        }

        private static void AddRefitWriters(ServiceCollection serviceCollection, 
                                            ApiWriterConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.Url))
                throw new Exception("ApiWriterConfiguration Url cannot be Empty!");

            serviceCollection.AddRefitClient<ITestApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(config.Url));
           
        }

        private static void AddRefitClients(ServiceCollection serviceCollection,
                                            TestConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.UtilitiesUrl))
                throw new Exception("UtilitiesUrl cannot be Empty!");

            serviceCollection.AddRefitClient<IUtilitiesApi>()
              .ConfigureHttpClient(c => c.BaseAddress = new Uri(config.UtilitiesUrl));
        }

    }
}
