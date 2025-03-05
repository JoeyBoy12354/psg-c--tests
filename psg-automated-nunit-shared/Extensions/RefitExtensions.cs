using Newtonsoft.Json;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.Models;
using Refit;

namespace psg_automated_nunit_shared.Extensions
{
    public static class RefitExtensions
    {
        public static bool CheckForContentType(this ApiResponse<string> response, string contentType)
        {
            var contentTypes = response.ContentHeaders?.SelectMany(x => x.Value) ?? [];

            var pdf = contentTypes?.Where(x => x.ToLower().Contains(contentType)) ?? [];

            return pdf.Any();
        }

        public static bool CheckSuccess(this ApiResponse<string> response)
        {
            var result = response.GetFicaResult();

            if (result == null || string.IsNullOrWhiteSpace(result.status))
                return false;

            if (result.status.ToLower() != "success")
                return false;

            return true;
        }

        public static FicaResult? GetFicaResult(this ApiResponse<string> response)
        {
            var result = response.GetContentAs<FicaResult>();

            return result;
        }

        public static T? GetContentAs<T>(this ApiResponse<string> response)
        {
            var content = response.Content;

            LogHelper.LogInfo("API Content returned: [{content}]", content);

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<T>(content);

                    if (result == null)
                        throw new ArgumentException($"token result is null after deserialisation");

                    return result;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("There was an error deserialising the Token! {message}", ex.Message);
                }
            }

            return default(T);
        }
    }
}
