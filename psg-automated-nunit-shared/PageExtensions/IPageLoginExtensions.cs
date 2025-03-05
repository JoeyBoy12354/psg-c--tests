using Microsoft.Playwright;
using NUnit.Framework;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Extensions;

namespace psg_automated_nunit_shared.PageExtensions
{
    public static class IPageLoginExtensions
    {
        private static float _loginTimeOut = 1000 * 120;        // 2 mins

        public static async Task LoginMyPracticeAsync(this IPage page, TestConfiguration config)
        {
            await page.GotoAsync(config.UrlMyPracticeLogin);
            await page.GetByLabel("Username").ClickAsync();
            await page.GetByLabel("Username").FillAsync(config.Username);
            await page.GetByLabel("Password").ClickAsync();
            await page.GetByLabel("Password").FillAsync(config.Password);
            await page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                                                                               .ClickAsync(new() { Timeout = _loginTimeOut });

            // check for errors
           
            for(int i = 0; i < 10; i++)
            {
                var error = await page.GetByRole(AriaRole.Heading, new() { Name = "Login error" }).IsVisibleAsync();

                if(error)
                {
                    throw new Exception("Error loging in to myPractice!");
                }

                var locked = await page.GetByRole(AriaRole.Heading, new() { Name = "Account locked" }).IsVisibleAsync();

                if(locked)
                {
                    throw new Exception("Account locked!!");
                }

                var loggedIn = await page.GetByLabel("Close").IsVisibleAsync();

                if (loggedIn)
                    break;

                await Task.Delay(1000);
            }

            try
            {
                // close message box
                await page.GetByLabel("Close").ClickAsync((new() { Timeout = 1000 }));
            }
            catch
            {
                try
                {
                    await page.GetByLabel("Close").First.ClickAsync(new() { Timeout = 1000 });
                }
                catch
                {
                    // do nothing
                }
            }

            await page.GetByRole(AriaRole.Heading, new() { Name = $"{config.Name} {config.Surname}" })
                                                   .IsVisibleAsync();

        }

        public static async Task GoToMyPracticeAsync(this IPage page,
                                                    TestConfiguration config,
                                                    IOtpService otpService)
        {
            var pageUrl = config.GetPageMyPractice();

            await page.GotoAsync(pageUrl);

            // somtimes you'll get redirected to myBase ....
            await page.CheckMyBaseAsync(config, otpService);
        }

        public static async Task<string> CheckMyBaseAsync(this IPage page,
                                                          TestConfiguration config,
                                                          IOtpService otpService)
        {
            for (int i = 0; i < 15; i++)
            {
                // we're in myPractice... return
                if (page.Url.Contains(config.UrlMyPractice))
                    return "";

                // check if the 'idNumber' button is showing
                var mustAuth = await page.GetByPlaceholder("ID number / Registration").IsVisibleAsync();

                if (mustAuth)
                {
                    await page.GetByPlaceholder("ID number / Registration").ClickAsync();
                    await page.GetByPlaceholder("ID number / Registration").FillAsync(config.Username);
                    await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
                    await page.GetByPlaceholder("Password").ClickAsync();
                    await page.GetByPlaceholder("Password").FillAsync(config.Password);

                    await page.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();

                    var result = await page.CheckMfaAsync(config, otpService);

                    if (!string.IsNullOrWhiteSpace(result))
                        Assert.Fail(result);

                    await GoToMyPracticeByClickAsync(page);

                    return "";
                }

                await Task.Delay(1000);
            }

            return "";
        }

        public static async Task GoToMyPracticeByClickAsync(this IPage page)
        {
            // the myPractice button can be on 2 different places
            try
            {
                await page.GetByText("myPractice").ClickAsync(new() { Timeout = _loginTimeOut });
            }
            catch
            {
                await page.GetByText("myPractice").First.ClickAsync(new() { Timeout = _loginTimeOut });
            }          

            // check for lockout
            await Task.Delay(1000);

            var lockout = await page.GetByRole(AriaRole.Heading, new() { Name = "Account locked" }).IsVisibleAsync();

            if (lockout)
            {
                Assert.Fail("Account is locked out!");
                return;
            }

            // check for open messagebox
            for(int i = 0; i < 10; i++)
            {
                var messageBox = await page.GetByRole(AriaRole.Button, new() { Name = "Close window" }).IsVisibleAsync();

                if (messageBox)
                {
                    await page.GetByRole(AriaRole.Button, new() { Name = "Close window" }).ClickAsync();
                    break;
                }
              
                await Task.Delay(1000);
            }           

        }

        public static async Task<string> LoginMyPsgAsync(this IPage page, TestConfiguration config, IOtpService otpService)
        {
            await page.GotoAsync(config.GetPageMyPsg("loginMyPsg"));

            await page.GetByPlaceholder("ID number / Registration").ClickAsync();
            await page.GetByPlaceholder("ID number / Registration").FillAsync(config.Username);
            await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
            await page.GetByPlaceholder("Password").ClickAsync();
            await page.GetByPlaceholder("Password").FillAsync(config.Password);

            await page.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();

            return await page.CheckMfaAsync(config, otpService);
        }

        public static async Task LoginMyBaseAsync(this IPage page, TestConfiguration config, IOtpService otpService)
        {
            await page.GotoAsync(config.GetPageMyBase("loginMyBase"));

            await page.GetByPlaceholder("ID number / Registration").ClickAsync();

            await page.GetByPlaceholder("ID number / Registration").FillAsync(config.Username);

            await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();

            await page.GetByPlaceholder("Password").ClickAsync();

            await page.GetByPlaceholder("Password").FillAsync(config.Password);

            await page.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();

            await page.CheckMfaAsync(config, otpService);
        }

        public static async Task LoginMfaTestAsync(this IPage page, TestConfiguration config, IOtpService otpService)
        {
            await page.GotoAsync(config.UrlMfa);

            await page.GetByPlaceholder("ID number / Registration").ClickAsync();
            await page.GetByPlaceholder("ID number / Registration").FillAsync(config.Username);
            await page.GetByRole(AriaRole.Button, new() { Name = "Next" })
                                                       .ClickAsync(new() { Timeout = _loginTimeOut });

            // MFA starts here
            await page.CheckMfaAsync(config, otpService);

        }

        // Checks if page was redirected to MFA OTP login and tries to login with Mfa
        public static async Task<string> CheckMfaAsync(this IPage page, TestConfiguration config, IOtpService otpService)
        {
            // wait a while          

            for (int i = 0; i < 15; i++)
            {
                // check if it's still the MFA page, if not, we don't need to authenticate
                if (!page.Url.Contains(config.UrlMfaCheck))
                    return "";

                try
                {
                    // check if the 'authenticate' button is showing...if it is, then we must authenticate
                    var mustAuth = await page.GetByRole(AriaRole.Button, new() { Name = "Authenticate" }).IsVisibleAsync();
    
                    if (mustAuth)
                        break;
                } catch { }

                await Task.Delay(1000);
            }

            // check if was redirected and do MFA if necessary
            if (page.Url.Contains(config.UrlMfaCheck))
            {
                return await page.LoginMfaAsync(config, otpService);
            }

            return "";
        }

        private static async Task<string> LoginMfaAsync(this IPage page, TestConfiguration config, IOtpService otpService)
        {
            // Get OTP - wait a bit until a new 1 is generated          

            for (int i = 0; i < 15; i++)
            {
                if (!page.Url.Contains(config.UrlMfaCheck))
                    return "";

                // check if the 'authenticate' button is showing...if it is, then we must authenticate
                var mustAuth = await page.GetByRole(AriaRole.Button, new() { Name = "Authenticate" }).IsVisibleAsync();

                if (mustAuth)
                    break;

                await Task.Delay(1000);
            }

            // check if already off the mfa page - measn we're authenticated already
            if (!page.Url.Contains(config.UrlMfaCheck))
                return "";

            var otp = await otpService.GetOtpAsync(page);

            if (!page.Url.Contains(config.UrlMfaCheck))
                return "";

            if (string.IsNullOrWhiteSpace(otp))
            {              
                return $"OTP is Empty!";
            }

            // Enter OTP
            await page.GetByLabel("Enter the code sent to your").ClickAsync();
            await page.GetByLabel("Enter the code sent to your").FillAsync(otp);

            await page.GetByRole(AriaRole.Button, new() { Name = "Authenticate" })
                                                                      .ClickAsync(new() { Timeout = _loginTimeOut });

            var failed = await page.GetByText("Failed").IsVisibleAsync();

            if (failed)
            {
                return $"OTP login failed! Otp used {otp}";
            }

            // page should be redirected back to were it came from

            return "";
        }

    }
}
