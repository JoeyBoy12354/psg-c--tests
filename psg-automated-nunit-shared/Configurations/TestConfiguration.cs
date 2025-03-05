namespace psg_automated_nunit_shared.Configurations
{
    /// <summary>
    /// Configuration to be used for testing, that is read from appsettings.json. 
    /// <br/> To be updated as needed.    
    /// </summary>
    public sealed class TestConfiguration
    {
        public bool Headless { get; init; }

        public string UrlMyBase { get; init; }
        public string UrlMyPsg { get; init; }
        public string UrlMyPractice { get; init; }
        public string UrlMyPracticeLogin { get; init; }

        public string UrlMfa { get; init; }
        public string UrlMfaCheck { get; init; }

        public string UtilitiesUrl { get; set; }
        public string Secret { get; set; }
        public string[] PhoneNumbers { get; set; }

        public string Username { get; init; }
        public string Password { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
        public string SearchName => $"{Name} {Surname}";
        public bool ErrorScreenshotsOnly { get; init; }
        public bool Screenshots { get; init; }
        public string ScreenshotPath { get; init; }

        public SaveConfiguration? SaveConfiguration { get; set; }

        public List<Page> Pages { get; init; } = [];

        public Dictionary<string, string>? pages = null;

    }

    public sealed record Page(string? Name, string? Url);


}
