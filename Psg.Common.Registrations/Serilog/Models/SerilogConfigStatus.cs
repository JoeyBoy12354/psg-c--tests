namespace Psg.Common.Registrations.Serilog.Models
{
    public class SerilogConfigStatus
    {
        public static SerilogConfigStatus Good => new();
        public static SerilogConfigStatus Bad(string message) => new(message);

        public bool ConfigValid { get; set; }
        public string Message { get; set; }



        private SerilogConfigStatus()
        {
            ConfigValid = true;
            Message = "";
        }

        private SerilogConfigStatus(string message)
        {
            ConfigValid = false;
            Message = message;
        }


    }
}
