namespace psg_automated_nunit_common.Configurations
{
    public sealed class ClientSecretConfiguration
    {
        public string Secret { get; set; } = string.Empty;
        public static ClientSecretConfiguration Empty => new(); 

        private ClientSecretConfiguration()
        {                
        }
    }
}
