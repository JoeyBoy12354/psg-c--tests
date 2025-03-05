using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Psg.Common.Registrations.Logging
{
    /// <summary>
    /// Gets a logger that can be used during startup
    /// </summary> 
    public static class LoggingExtensions
    {

        /// <summary>
        /// Gets a logger that can be used during startup
        /// </summary>  
        public static ILogger<T> GetStartupLogger<T>(this WebApplicationBuilder builder)
        {
            var sp = builder.Services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();

            return logger;
        }

        /// <summary>
        /// Gets a logger that can be used during startup
        /// </summary>  
        public static ILogger GetStartupLogger(this WebApplicationBuilder builder)
        {
            var sp = builder.Services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<StaticLogger>>();

            return logger;
        }

        /// <summary>
        /// Gets a logger that can be used during startup
        /// </summary>  
        public static ILogger GetStartupLogger(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<StaticLogger>>();

            return logger;
        }

    }
}