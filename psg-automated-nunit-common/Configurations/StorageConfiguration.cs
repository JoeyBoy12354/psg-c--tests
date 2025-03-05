namespace psg_automated_nunit_common.Configurations
{
    public sealed class StorageConfiguration
    {
        public bool SaveResults { get; init; }
        public List<object>? WriteTo { get; set; }
    }
}
