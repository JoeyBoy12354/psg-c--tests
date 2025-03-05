using psg_automated_nunit_shared.Contracts;

namespace psg_automated_nunit_shared.Configurations
{
    /// <summary>
    /// Configuration for Digital Fica
    /// </summary>
    public sealed class FicaApiConfiguration
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IDNumber { get; set; }
        public string EventID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MatchingDetailsId { get; set; }

    }
}
