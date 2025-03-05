using Microsoft.Extensions.DependencyInjection;
using Psg.Common.Registrations.Serilog;
using psg_automated_nunit_shared.Extensions;
using psg_automated_nunit_shared.Helpers;

namespace psg_automated_nunit_shared.Managers
{
    /// <summary>
    /// Manages Dependency injection and services. This class runs before any of the tests start.
    /// </summary>
    public static class DependencyManager
    {
        private const string ApplicationName = "Automated";

        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// Loads the appsettings and sets up any services
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider Configure()
        {
            if (_serviceProvider != null)
                return _serviceProvider;
          
            var services = new ServiceCollection();

            // Configure the configuration
            var configuration = ConfigurationHelper.GetConfiguration();

            // so that serilog can write it in the finally block
            configuration.AddSerilogPsg(ApplicationName, ConfigurationHelper.Environment);           

            services.AddTestConfiguration(configuration);
            services.AddFicaApiConfiguration(configuration);         


            services.AddSingleton<ScreenshotManager>();
         

            _serviceProvider = services.BuildServiceProvider();
            

            return _serviceProvider;
        }    
        


        /// <summary>
        /// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>     
        /// <returns>A service object of type <typeparamref name="T"/>.</returns>
        public static T GetRequiredService<T>() where T : class, new()
        {
            if (_serviceProvider == null)
                Configure();

            if (_serviceProvider == null)
                return new();

            return _serviceProvider.GetRequiredService<T>() ?? new();
        }

        /// <summary>
        /// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>      
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        public static T? GetService<T>() where T : class
        {
            if (_serviceProvider == null)
                Configure();

            if (_serviceProvider == null)
                return default;

            return _serviceProvider.GetService<T>();
        }

        /// <summary>
        /// Get a List of services of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>       
        /// <returns>An enumeration of services of type <typeparamref name="T"/>.</returns>
        public static List<T> GetServices<T>() where T : class
        {
            if (_serviceProvider == null)
                Configure();

            if (_serviceProvider == null)
                return new();

            return _serviceProvider.GetServices<T>()?.ToList() ?? [];
        }



    }
}
