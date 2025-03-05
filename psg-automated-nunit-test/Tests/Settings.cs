using Microsoft.Playwright;
using Polly;
using Psg.Common.Registrations.Polly.Policies;
using psg_automated_nunit_shared.Attributes;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Contracts.Base;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.PageExtensions;

namespace psg_automated_nunit_test.Tests
{

    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!


    //[Parallelizable(ParallelScope.Fixtures)]
    [TestFixture, Order(8)]
    public sealed class Settings : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;


        public Settings() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }



        [Test]
        [Prod]
        public async Task Settings_Countersign_On_My_Behalf_Pagination()
        {
            // arrange
            Name = "Settings - Countersign on my Behalf Pagination";

            Description = "Log in Johan Website ( role Administrator) then search and login as Client Care Centre, PSG Client Care Centre , go to settings and then preferences. " +
                          "Click on Countersign on my behalf. Ensure that you can navigate between the pages.";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            // act

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await page.GoToMyPracticeAsync(config, _otpService);

                    //search for Client Care Centre

                    await page.Locator("[id=\"__search\"]").ClickAsync();
                    await page.Locator("[id=\"__search\"]").FillAsync("Client Care Centre");
                    await Task.Delay(3000);
                    await page.GetByText("Client Care Centre in Clients").ClickAsync();
                    await Task.Delay(5000);
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 Client Care Centre, PSG" }).Locator("svg").ClickAsync();

                    // wait for the screens to load, by waiting for some random header
                    await page.GetByRole(AriaRole.Heading, new() { Name = "Summary" }).ClickAsync();

                    // login as
                    await page.GetByRole(AriaRole.Button, new() { Name = "More actions" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Login as" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();

                    // wait for the screens to load
                    await page.GetByRole(AriaRole.Heading, new() { Name = "Outstanding notes" }).ClickAsync();

                    // click on Logged in as, to see menu
                    await page.GetByRole(AriaRole.Heading, new() { Name = "PSG Konsult Client Care" }).ClickAsync();

                    // click Settings in Menu
                    await page.GetByText("Settings").ClickAsync();

                    await page.Locator("a").Filter(new() { HasText = "Preferences" }).ClickAsync();

                    await Task.Delay(5000);
                    var visible = await page.GetByPlaceholder("-- Not Selected --").IsVisibleAsync();

                    if (visible)
                    {
                        await page.GetByPlaceholder("-- Not Selected --").ClickAsync();
                        await page.GetByRole(AriaRole.Option, new() { Name = "Short Term Insurance (Personal)" }).ClickAsync();
                        await page.GetByText("Countersign on my behalf").ClickAsync();
                        await page.GetByRole(AriaRole.Searchbox).ClickAsync();
                        await page.GetByText("NoYesDocument workflows with").ClickAsync();
                        await page.GetByText("Countersign on my behalf").ClickAsync();
                        await page.GetByRole(AriaRole.Button, new() { Name = "Save changes" }).ClickAsync();
                        await Task.Delay(5000);
                    }
                    await page.GetByText("Countersign on my behalf").ClickAsync();
                    await page.Locator("a").Filter(new() { HasText = "Short Term Insurance (Personal)" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "2" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "3" }).ClickAsync();

                });



            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }

        }
       
    }
}