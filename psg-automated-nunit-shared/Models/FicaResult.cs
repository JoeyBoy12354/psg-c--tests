namespace psg_automated_nunit_shared.Models
{
    public class FicaResult
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<object> validationErrors { get; set; }
        public object result { get; set; }
        public string error { get; set; }
    }


}
