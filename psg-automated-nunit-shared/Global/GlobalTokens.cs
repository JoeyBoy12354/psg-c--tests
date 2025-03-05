using Newtonsoft.Json;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Helpers;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.Models;
using psg_automated_nunit_shared.Refit;

namespace psg_automated_nunit_shared.Global
{
    public class GlobalTokens
    {
        private readonly TestConfiguration _testConfig;  
        private readonly FicaApiConfiguration _ficaApiConfig;
        private readonly IAzureExternalTokenProviderApi? _azureExternalTokenProviderApi;

        private GlobalTokens()
        {
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _ficaApiConfig = DependencyManager.GetRequiredService<FicaApiConfiguration>();
            _azureExternalTokenProviderApi = DependencyManager.GetService<IAzureExternalTokenProviderApi>();
        }

        public static GlobalTokens Create()
        {
            return new GlobalTokens();
        }
    
        /// <summary>
        /// Run all reusable Login and Token methods here
        /// </summary>
        /// <returns></returns>
        public async Task<GlobalTokens> SetupAsync()
        {         
            await GenerateFicaTokenAsync();     

            return this;
        }
   

        public async Task GenerateFicaTokenAsync()
        { 
            try
            {
                if (_azureExternalTokenProviderApi == null)
                {
                    LogHelper.LogError("There was an error uploading the results to the api! _azureExternalTokenProviderApi interface was null!");
                    return;
                }

                LogHelper.LogInfo("Getting token...");

                var headers = new Dictionary<string, string>
                {
                    //{ "X-Api-Secret", _apiWriterConfiguration.Secret }
                };

                GenerateTokenModel model = new()
                {
                    username = _ficaApiConfig.Username,
                    password = _ficaApiConfig.Password,
                };

                var response = await _azureExternalTokenProviderApi.GenerateTokenAsync(headers, model);

                if (!response.IsSuccessStatusCode)
                {
                    LogHelper.LogError("There was an error getting the Token! {message}", response.Error?.Message);
                }

                var content = response.Content;

                LogHelper.LogInfo("API Content returned: [{content}]", content);

                if(!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<FicaTokenResult>(content);

                        if (result == null)
                            throw new ArgumentException($"token result is null after deserialisation");

                        PlaywrightManager.FicaToken = result.result.access_token;
                    }
                    catch(Exception ex) 
                    {
                        LogHelper.LogError("There was an error deserialising the Token! {message}", ex.Message);
                    }
                     
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }


        }

    }
}
