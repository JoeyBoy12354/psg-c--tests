namespace Psg.Standardised.Api.Common.Configurations
{
    public sealed class CacheConfiguration
    {
        public int SizeLimit { get; set; }
        public int AbsoluteExpirationHours { get; set; }

        public static CacheConfiguration Empty => new();

        private CacheConfiguration()
        {                
        }
    }
}
