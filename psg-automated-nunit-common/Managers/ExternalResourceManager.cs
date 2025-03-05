using psg_automated_nunit_common.Configurations;

namespace psg_automated_nunit_common.Managers
{
    public sealed class ExternalResourceManager
    {

        private readonly Dictionary<string, string> _files = [];

        public int FilesCount => _files.Count;

        public ExternalResourceManager(ExternalResourceConfiguration config)
        {
            foreach(var c in config.Configurations)
            {
                _files[c.Key.ToLower()] = c.Value;
            }
        } 

     
        public string GetLink(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            if (_files.TryGetValue(resourceName.ToLower(), out var link))
            {               
                return link;
            }

            return "";
        }
    }
}
