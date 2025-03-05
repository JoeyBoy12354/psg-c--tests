using System.Reflection;

namespace Psg.Common.Registrations.Version
{
    /// <summary>
    /// Gets the Psg.Common.Registrations version
    /// </summary>
    public static class RegistrationsVersion
    {
        /// <summary>
        /// The Psg.Common.Registrations version
        /// </summary>
        public static string Version => _version;

        /// <summary>
        /// The name Psg.Common.Registrations
        /// </summary>
        public static string Name => _name;

        private static readonly string _version = GetVersion();
        private static readonly string _name = GetName();

        /// <summary>
        /// Adds the Psg.Common.Registrations version to the InfoController.
        /// </summary>
        /// <param name="infoDictionary"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddPsgCommonRegistrationsVersion(this Dictionary<string, string> infoDictionary)
        {
            infoDictionary ??= new();

            var versionDic = GetVersionDictionary();

            foreach (var (key, value) in versionDic)
            {
                infoDictionary.TryAdd(key, value);
            }

            return infoDictionary;
        }

        private static Dictionary<string, string> GetVersionDictionary(bool appendVersion = false)
        {
            try
            {
                var name = GetName();

                if (appendVersion)
                    name = $"{name} Version";

                var version = GetVersion();

                return new() { { name, version } };
            }
            catch
            {
                return [];
            }
        }


        private static string GetVersion()
        {
            try
            {
                var assembly = typeof(RegistrationsVersion).Assembly;
                var attr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                var value = attr?.InformationalVersion;
                var array = value?.Split('+') ?? [];

                if (array.Length > 0)
                    return array[0];

                return "Not Found";
            }
            catch
            {
                return "Not Found";
            }
        }

        private static string GetName()
        {
            try
            {
                var assembly = typeof(RegistrationsVersion).Assembly;
                var assembleName = assembly.GetName();
                var name = assembleName.Name; 

                return name ?? "Not Found";
            }
            catch
            {
                return "Not Found";
            }
        }

       
    }
}
