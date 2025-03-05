using Microsoft.Extensions.Hosting;

namespace Psg.Common.Registrations.Extensions
{
    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Ckecks if Environment is any kind of local environment.
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        public static bool IsLocal(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.EnvironmentName.ToLower().Contains("local");
        }

        /// <summary>
        /// Checks if Environment is NOT local.
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        public static bool IsNotLocal(this IHostEnvironment hostEnvironment)
        {
            return !hostEnvironment.IsLocal();
        }
    }
}
