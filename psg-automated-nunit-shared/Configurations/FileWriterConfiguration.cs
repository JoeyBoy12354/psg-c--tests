using psg_automated_nunit_shared.Contracts;

namespace psg_automated_nunit_shared.Configurations
{
    /// <summary>
    /// Configuration to write test results to file.
    /// </summary>
    public sealed class FileWriterConfiguration: IWriterConfiguration
    {
        public bool Enabled { get; set; } = true;
        public string? Folder { get; set; }
        
    }  
}
