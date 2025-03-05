namespace Psg.Common.Registrations.Serilog.Models
{

    /// <summary>
    /// Just a general validation model to check if the Serilog config was found in appsettings
    /// </summary>
    public class SerilogValidationModel
    {
        public List<string> Using { get; set; }
        public string MinimumLevel { get; set; }
        public List<WriteTo> WriteTo { get; set; }
        public List<string> Enrich { get; set; }
    }

    public class Args
    {
        public List<Configure> configure { get; set; }
        public string serverUrl { get; set; }
        public string path { get; set; }
        public bool shared { get; set; }
        public string rollingInterval { get; set; }
        public bool rollOnFileSizeLimit { get; set; }
        public int fileSizeLimitBytes { get; set; }
        public int retainedFileCountLimit { get; set; }
    }

    public class Configure
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }



    public class WriteTo
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }
}
