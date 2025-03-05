namespace Psg.Standardised.Api.Common.Models
{
    public class CoreResult<T>
    {
        public T Result { get; set; }

        public bool ChangeLogged { get; set; }

        public string ChangeName { get; set; }

        public string ChangeLogId { get; set; }

        public string ChangeLogErrorMessage { get; set; }

        public Dictionary<string, string> GetChangeData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add(nameof(ChangeLogged), ChangeLogged.ToString());
            if (!ChangeLogged)
            {
                data.Add(nameof(ChangeLogErrorMessage), ChangeLogErrorMessage);
            }

            if (!string.IsNullOrEmpty(ChangeName))
            {
                data.Add(ChangeName, ChangeLogId);
            }

            return data;
        }
    }
}
