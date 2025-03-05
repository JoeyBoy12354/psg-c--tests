using Humanizer;

namespace psg_automated_nunit_shared.Helpers
{
    /// <summary>
    /// Helps to get string keys.
    /// </summary>
    public static class KeyHelper
    {
        /// <summary>
        /// Gets a string key from the displayName and methodName.
        /// </summary>   
        /// <returns></returns>
        public static string GetKey(string? displayName, string? methodName)
        {
            // var key = $"{displayName}_{methodName}";
            // 
            var key = methodName.Humanize();

            return key;
        }
    }
}
