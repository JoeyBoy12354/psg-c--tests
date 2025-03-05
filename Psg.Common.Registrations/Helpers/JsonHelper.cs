using Newtonsoft.Json;

namespace Psg.Common.Registrations.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Use this to fix json for display purposes.
        /// </summary>
        /// <param name="dynamicObj"></param>
        /// <param name="fieldName"></param>
        public static void CleanJsonFieldInDynamicObject(dynamic dynamicObj, string fieldName)
        {
            var decodedField = dynamicObj?[fieldName];

            if (decodedField != null)
            {
                string value = decodedField.ToString();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    var decodedJson = JsonConvert.DeserializeObject<dynamic>(value);
                    dynamicObj![fieldName] = decodedJson;
                }
            }
        }
    }
}
