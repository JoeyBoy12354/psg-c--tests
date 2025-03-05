using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Psg.Common.Registrations.Logging;
using Psg.Common.Registrations.Polly.Policies;
using System.Net.Http.Headers;

namespace Psg.Common.Registrations.Clients
{
    public static class ClientExtensions
    {

        /// <summary>
        /// Registers a PSgApiClient and it's config.
        /// <br/> Assumes Appsettings are in format {Name} without 'Client' in text, and Url is under the 'Api' field.
        /// <br/> Contains overloads to override the JsonPaths.
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TClientConfig"></typeparam>
        /// <param name="builder"></param>
        /// <param name="sectionKey">Name of the Section header for the Client in the Appsettings. <br/>If empty, default settings will be used</param>
        /// <param name="urlKey">Name of the field which contains the Url. <br/>If empty, defualt fieldname will be used</param>
        /// <param name="sectionConfigKey">Name of the Section header for the Config in the Appsettings. <br/>If empty, default settings will be used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static WebApplicationBuilder RegisterHttpClientAndConfig<TClient, TClientConfig>(this WebApplicationBuilder builder,
                                                                                                string? sectionKey = default,
                                                                                                string? urlKey = "Api",
                                                                                                string? secondApiKey = "Url",
                                                                                                string? sectionConfigKey = default
                                                                                                )
                                                                                                where TClient : class
                                                                                                where TClientConfig : class, new()

        {
            var clientType = typeof(TClient);

            var clientName = clientType.Name
                ?? throw new ArgumentException($"clientName null!");

            if (string.IsNullOrEmpty(sectionKey))
            {
                sectionKey = clientName.Replace("Client", "");
            }

            if (string.IsNullOrEmpty(sectionConfigKey))
            {
                // sectionConfigKey = $"{sectionKey}Config";
                sectionConfigKey = sectionKey;
            }

            // try to register config, if client comes with a clientconfig
            RegisterClientConfig<TClientConfig>(builder, sectionConfigKey);

            // get Url from Appsettings
            string? url = GetValueFromAppSettings(builder, sectionKey, urlKey, secondApiKey);

            RegisterClient<TClient>(builder, url);

            return builder;
        }

        /// <summary>
        /// Register a PSG Api client
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="builder"></param>
        /// <param name="sectionKey"></param>
        /// <param name="urlKey"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void RegisterHttpClient<TClient>(this WebApplicationBuilder builder,
                                                         string? sectionKey = default,
                                                         string? urlKey = "Api",
                                                          string? secondApiKey = "Url"
                                                         ) where TClient : class
        {
            var clientType = typeof(TClient);

            var clientName = clientType.Name
                ?? throw new ArgumentException($"clientName null!");

            if (string.IsNullOrEmpty(sectionKey))
            {
                sectionKey = clientName.Replace("Client", "");
            }

            // get Url from Appsettings
            string? url = GetValueFromAppSettings(builder, sectionKey, urlKey, secondApiKey);

            RegisterClient<TClient>(builder, url);

        }


        private static void RegisterClient<TClient>(WebApplicationBuilder builder,
                                                string url
                                                ) where TClient : class
        {
            var logger = builder.GetStartupLogger<TClient>();

            builder.Services.AddHttpClient<TClient>(options =>
            {
                options.BaseAddress = new Uri(url);
                options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(PollyPolicies.GetRetryPolicy());

            logger.LogInformation("{Name} Registered", typeof(TClient).Name);

        }

        private static void RegisterClientConfig<TClientConfig>(WebApplicationBuilder builder,
                                                                string sectionConfigKey
                                                                ) where TClientConfig : class, new()
        {
            var logger = builder.GetStartupLogger<TClientConfig>();

            try
            {
                var typeClientConfig = new TClientConfig();
                builder.Configuration.GetSection(sectionConfigKey).Bind(typeClientConfig);   

                builder.Services.AddSingleton(typeClientConfig);

                logger.LogInformation("{SectionConfigKey} Registered", sectionConfigKey);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering config: {Message}", ex.Message);
            }
        }

        private static string GetValueFromAppSettings(WebApplicationBuilder builder,
                                                    string sectionKey,
                                                    string? valueKey,
                                                    string? secondValueKey = default)
        {
            var logger = builder.GetStartupLogger();

            var jsonKey = $"{sectionKey}:{valueKey}";

            var value = GetValueFromAppSettingsByJsonKey(builder, jsonKey);

            if (string.IsNullOrEmpty(value))
            {
                logger.LogError("Client Registration Error: No string value found for [{Section}]", jsonKey);

                if (!string.IsNullOrWhiteSpace(secondValueKey))
                {
                    jsonKey = $"{jsonKey}:{secondValueKey}";

                    logger.LogInformation("Looking for value in [{Section}] ...", jsonKey);

                    value = GetValueFromAppSettingsByJsonKey(builder, jsonKey);

                    if (string.IsNullOrEmpty(value))
                    {
                        logger.LogInformation("Value found in [{Section}]", jsonKey);
                    }
                }
            }

            if (string.IsNullOrEmpty(value))
            {
                logger.LogError("Client Registration Error: No string value found for [{Section}]", jsonKey);
                logger.LogError("Ending Application with Environment.Exit(1)");
                Environment.Exit(1);
            }  

            return value;
        }

        private static string? GetValueFromAppSettingsByJsonKey(WebApplicationBuilder builder,
                                                               string jsonKey)
        {
            var value = builder.Configuration.GetValue<string>(jsonKey);          

            return value;
        }
    }
}
