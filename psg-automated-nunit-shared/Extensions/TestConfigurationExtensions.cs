using psg_automated_nunit_shared.Configurations;

namespace psg_automated_nunit_shared.Extensions
{
    public static class TestConfigurationExtensions
    {
        /// <summary>
        /// Gets a url by name, from the <c>Pages</c> list in the <c>appsettings.json</c>, 
        /// <br/> and prepends it with the <c>UrlMyPractice</c> in the <c>appsettings.json</c>
        /// </summary>       
        /// <returns></returns>
        public static string GetPageMyPractice(this TestConfiguration config, string? name = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"{config.UrlMyPractice}";

            var page = GetPage(config, name);

            if (string.IsNullOrWhiteSpace(page))
                return "";

            return $"{config.UrlMyPractice}{page}";
        }

        /// <summary>
        /// Gets a url by name, from the <c>Pages</c> list in the <c>appsettings.json</c>, 
        /// <br/> and prepends it with the <c>UrlMyBase</c> in the <c>appsettings.json</c>
        /// </summary>       
        /// <returns></returns>
        public static string GetPageMyBase(this TestConfiguration config, string name)
        {
            var page = GetPage(config, name);

            if (string.IsNullOrWhiteSpace(page))
                return "";

            return $"{config.UrlMyBase}{page}";
        }

        /// <summary>
        /// Gets a url by name, from the <c>Pages</c> list in the <c>appsettings.json</c>, 
        /// <br/> and prepends it with the <c>UrlMyPsg</c> in the <c>appsettings.json</c>
        /// </summary>       
        /// <returns></returns>
        public static string GetPageMyPsg(this TestConfiguration config, string name)
        {
            var page = GetPage(config, name);

            if (string.IsNullOrWhiteSpace(page))
                return "";

            return $"{config.UrlMyPsg}{page}";
        }

        private static string GetPage(TestConfiguration config, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            if (config.pages == null)
                config.pages = config.Pages?.ToDictionary(x => x.Name ?? "", x => x.Url ?? "") ?? [];

            if (!config.pages.TryGetValue(name, out var value))
                return "";

            return value;
        }
    }
}
