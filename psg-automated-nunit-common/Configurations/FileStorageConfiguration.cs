using psg_automated_nunit_common.Contracts;

namespace psg_automated_nunit_common.Configurations
{
    /// <summary>
    /// Configuration to write test results to file.
    /// </summary>
    public sealed class FileStorageConfiguration: IStorageConfiguration
    {
        public bool Enabled { get; set; } = true;
        public string? Folder { get; set; }
        
    }  
}
