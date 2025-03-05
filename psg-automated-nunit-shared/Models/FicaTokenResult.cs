namespace psg_automated_nunit_shared.Models
{
    public class FicaTokenResult
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<object> validationErrors { get; set; }
        public Result result { get; set; }
        public string error { get; set; }
    }
    public class Result
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string resource { get; set; }
        public string refresh_token { get; set; }
        public int refresh_token_expires_in { get; set; }
        public string scope { get; set; }
        public string id_token { get; set; }
        public DateTime expiryDate { get; set; }
        public DateTime canRefreshDate { get; set; }
    }

  

}
