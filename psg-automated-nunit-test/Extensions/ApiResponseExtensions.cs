using Refit;

namespace psg_automated_nunit_test.Extensions
{
    public static class ApiResponseExtensions
    {
        public static string GetErrorMessage(this ApiResponse<string>? response) 
        {
            if (response == null)
                return "";


            var message = response.Error?.Message;

            if(!string.IsNullOrEmpty(response.Content))
            {
                message += response.Content;
            }

            if (!string.IsNullOrEmpty(response.Error?.Content))
            {
                message += response.Error.Content;
            }

            return message ?? "";
        }
    }
}
