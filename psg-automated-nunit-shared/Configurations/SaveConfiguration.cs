namespace psg_automated_nunit_shared.Configurations
{
    /// <summary>
    /// Configuration needed to save test results.
    /// </summary>
    public sealed class SaveConfiguration
    {
        public bool SaveResults { get; init; }
        public List<object>? WriteTo { get; set; }
    }
}
