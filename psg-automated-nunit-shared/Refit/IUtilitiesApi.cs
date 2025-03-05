using Refit;

namespace psg_automated_nunit_shared.Refit
{
    public interface IUtilitiesApi
    {
        [Get($"/OTP")]
        Task<ApiResponse<string>> OtpAsync([HeaderCollection] IDictionary<string, string> headers,
                                           string phoneNumber);

        [Get($"/OTPS")]
        Task<ApiResponse<string>> OtpsAsync([HeaderCollection] IDictionary<string, string> headers,
                                            string[] phoneNumbers);

        [Get($"/OTPD")]
        Task<ApiResponse<string>> OtpDownloadAsync([HeaderCollection] IDictionary<string, string> headers,
                                           string[] phoneNumbers);
    }
}
