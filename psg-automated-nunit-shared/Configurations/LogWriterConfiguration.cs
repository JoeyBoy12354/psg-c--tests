using psg_automated_nunit_shared.Contracts;

namespace psg_automated_nunit_shared.Configurations
{
    /// <summary>
    /// Configuration to write test results to SEQ.
    /// </summary>
    public sealed class LogWriterConfiguration : IWriterConfiguration
    {
        public bool Enabled { get; set; } = true;
        
    }  
}
