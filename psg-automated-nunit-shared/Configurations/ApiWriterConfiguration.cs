using psg_automated_nunit_shared.Contracts;

namespace psg_automated_nunit_shared.Configurations
{
    /// <summary>
    /// Configuration to save Results to Api
    /// </summary>
    public sealed class ApiWriterConfiguration : IWriterConfiguration
    {
        public bool Enabled { get; set; } = true;
        public string Url { get; set; }
        public string Secret { get; set; }

    }  
}
