using Microsoft.Playwright;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.PageExtensions;

namespace psg_automated_nunit_shared.Global
{
    public class GlobalLogins
    { 

        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;

        private GlobalLogins()
        {
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }

        public static GlobalLogins Create()
        {
            return new GlobalLogins();
        }
    
        /// <summary>
        /// Run all reusable Login and Token methods here
        /// </summary>
        /// <returns></returns>
        public async Task<GlobalLogins> SetupAsync()
        {           
            await LoginMyPracticeAsync();

            return this;
        }
   
        public async Task LoginMyPracticeAsync()
        {
            PlaywrightManager.ClearLoginState();

            var context = await PlaywrightManager.GetNewBrowserContextAsync();

            var page = await context.NewPageAsync();          

            try
            {
                await page.LoginMyPracticeAsync(_testConfig);

                await PlaywrightManager.SaveLoginStateAsync(context);
                PlaywrightManager.IsLoggedIn = true;

                await PlaywrightManager.ClearContextsAsync();              

            }            
            finally
            {             
                await context.CloseAsync();
            }
        }     

        public async Task LoginMyPsgAsync()
        {
            PlaywrightManager.ClearLoginState();

            var context = await PlaywrightManager.GetNewBrowserContextAsync();

            var page = await context.NewPageAsync();

            try
            {
                await page.LoginMyPsgAsync(_testConfig, _otpService);

                await PlaywrightManager.SaveLoginStateAsync(context);
                PlaywrightManager.IsLoggedIn = true;

                await PlaywrightManager.ClearContextsAsync();

            }
            finally
            {
                await context.CloseAsync();
            }
        }

    }
}
