namespace psg_automated_nunit_common.Configurations
{
    public sealed class AuthorisationConfiguration
    {
        public string Api { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string CachingPolicy { get; set; } = string.Empty;

        public static AuthorisationConfiguration Empty => new();

        private AuthorisationConfiguration()
        {
        }
    }
}
