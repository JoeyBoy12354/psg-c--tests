using Microsoft.Playwright;
using Newtonsoft.Json;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Refit;

namespace psg_automated_nunit_shared.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUtilitiesApi _utilitiesApi;
        private readonly TestConfiguration _testConfiguration;

        public OtpService(IUtilitiesApi utilitiesApi,
                          TestConfiguration testConfiguration)
        {
            _utilitiesApi = utilitiesApi;
            _testConfiguration = testConfiguration;
        }

        public async Task<string> GetOtpAsync(IPage page, string phoneNumber = "", int maxRetries = 10)
        {
            try
            {
                if (!page.Url.Contains(_testConfiguration.UrlMfaCheck))
                    return "";

                string[] numbers = [];

                // check if number was overridden
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    numbers = [phoneNumber];
                }

                if (numbers.Length == 0)
                {
                    // set default number from config
                    numbers = _testConfiguration.PhoneNumbers;
                }

                if (numbers.Length == 0)
                {
                    LogHelper.LogError("There was an error getting OTP! phoneNumber was null!");
                    return "";
                }

                // loop until OTP found

                string otp = "";
                int count = 0;

                while (string.IsNullOrWhiteSpace(otp) && count < maxRetries)
                {
                    try
                    {
                        if (!page.Url.Contains(_testConfiguration.UrlMfaCheck))
                            return "";

                        otp = await OtpCallAsync(numbers);

                        if (!string.IsNullOrEmpty(otp))
                            return otp;

                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex, "There was an error getting OTP!");
                        throw;
                    }
                    finally
                    {
                        count++;
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }

            return "";
        }

        public async Task<string> GetOtpDownloadAsync(IPage page, string phoneNumber = "", int maxRetries = 10)
        {
            try
            {
                string[] numbers = [];

                // check if number was overridden
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    numbers = [phoneNumber];
                }

                if (numbers.Length == 0)
                {
                    // set default number from config
                    numbers = _testConfiguration.PhoneNumbers;
                }

                if (numbers.Length == 0)
                {
                    LogHelper.LogError("There was an error getting OTP! phoneNumber was null!");
                    return "";
                }

                // loop until OTP found

                string otp = "";
                int count = 0;

                while (string.IsNullOrWhiteSpace(otp) && count < maxRetries)
                {
                    try
                    {
                        otp = await OtpDownloadCallAsync(numbers);

                        if (!string.IsNullOrEmpty(otp))
                            return otp;

                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex, "There was an error getting OTP!");
                        throw;
                    }
                    finally
                    {
                        count++;
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }

            return "";
        }

        private async Task<string> OtpCallAsync(string[] phoneNumbers)
        {
            try
            {
                if (phoneNumbers.Length == 0)
                {
                    LogHelper.LogError("There was an error getting OTP! phoneNumber was null!");
                    return "";
                }

                if (_utilitiesApi == null)
                {
                    LogHelper.LogError("There was an error getting OTP! _utilitiesApi interface was null!");
                    return "";
                }

                LogHelper.LogInfo("Getting OTP for {PhoneNumber}", phoneNumbers);

                var headers = new Dictionary<string, string>
                {
                    { "X-Api-Secret", _testConfiguration.Secret }
                };

                var response = await _utilitiesApi.OtpsAsync(headers, phoneNumbers);

                if (!response.IsSuccessStatusCode)
                {
                    var path = response.RequestMessage?.RequestUri?.ToString();

                    LogHelper.LogInfo("API Content returned: [{content}]", response.Content);
                    LogHelper.LogInfo("API Error: [{Error}]", response.Error);
                    throw new Exception($"{response.Error}");
                }


                var content = response.Content;

                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new Exception($"No Content returned by OTP call");
                }

                var result = JsonConvert.DeserializeObject<OtpResult>(content);

                if (result == null)
                {
                    throw new Exception($"Result after deserialisation is empty");
                }

                if (!result.IsSuccess)
                {
                    LogHelper.LogInfo("API OTP status does not indicate success for some reason");
                }

                return result.Message;

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                throw;
            }

            return "";
        }

        private async Task<string> OtpDownloadCallAsync(string[] phoneNumbers)
        {
            try
            {
                if (phoneNumbers.Length == 0)
                {
                    LogHelper.LogError("There was an error getting OTP! phoneNumber was null!");
                    return "";
                }

                if (_utilitiesApi == null)
                {
                    LogHelper.LogError("There was an error getting OTP! _utilitiesApi interface was null!");
                    return "";
                }

                LogHelper.LogInfo("Getting OTP for {PhoneNumber}", phoneNumbers);

                var headers = new Dictionary<string, string>
                {
                    { "X-Api-Secret", _testConfiguration.Secret }
                };

                var response = await _utilitiesApi.OtpDownloadAsync(headers, phoneNumbers);

                if (!response.IsSuccessStatusCode)
                {
                    var path = response.RequestMessage?.RequestUri?.ToString();

                    LogHelper.LogInfo("API Content returned: [{content}]", response.Content);
                    LogHelper.LogInfo("API Error: [{Error}]", response.Error);
                    throw new Exception($"{response.Error}");
                }


                var content = response.Content;

                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new Exception($"No Content returned by OTP call");
                }

                var result = JsonConvert.DeserializeObject<OtpResult>(content);

                if (result == null)
                {
                    throw new Exception($"Result after deserialisation is empty");
                }

                if (!result.IsSuccess)
                {
                    LogHelper.LogInfo("API OTP status does not indicate success for some reason");
                }

                return result.Message;

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                throw;
            }

            return "";
        }
    }




    public record OtpResult(bool IsSuccess, string Message);
}
