namespace Psg.Standardised.Api.Common.Configurations
{
    public sealed class TestCacheConfiguration
    {      
        public int AbsoluteExpirationHours { get; set; }

        public static TestCacheConfiguration Empty => new();

        private TestCacheConfiguration()
        {                
        }
    }
}
