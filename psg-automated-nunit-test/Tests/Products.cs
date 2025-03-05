using Microsoft.Playwright;
using Polly;
using Psg.Common.Registrations.Polly.Policies;
using psg_automated_nunit_shared.Attributes;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Contracts.Base;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.PageExtensions;
using System.Reflection;

namespace psg_automated_nunit_test.Tests
{

    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!


    //[Parallelizable(ParallelScope.Fixtures)]
    [TestFixture, Order(5)]
    public sealed class Products : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly int _timeOutMs = 120_000;
        private readonly IOtpService _otpService;

        public Products() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }
       

        [Test]
        [Prod]
        public async Task Products_Add_New_Product_Button()
        {
            // arrange
            Name = "Products - Add new Product button to be available";

            Description = "Add new button to be available on the Products tab on a client profile.";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            // act

            try
            {
                //await _retryPolicy.ExecuteAsync(async () =>
                //{
                    await page.GoToMyPracticeAsync(config, _otpService);

                    await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                       .ClickAsync(new() { Timeout = _timeOutMs });

                    await page.GetByText("Financial Adviser").ClickAsync();
                    await page.GetByText("Clients", new() { Exact = true }).First.ClickAsync();
                    await page.GetByText("Clients", new() { Exact = true }).Nth(2).ClickAsync();
                    await page.GetByPlaceholder("Find").Nth(0).ClickAsync();
                    await page.GetByPlaceholder("Find").Nth(0).FillAsync("0103045694082");
                    await page.GetByPlaceholder("Find").Nth(0).PressAsync("Enter");
                    await page.GetByText("0103045694082 in Clients").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 Client, Automation" }).Locator("svg").ClickAsync();
                //Dismiss Modal if present
                //var modalPopup = page.Locator("#popup_1");
                //if(await modalPopup.IsVisibleAsync())
                //{
                //    await page.GetByLabel("Close").IsVisibleAsync();
                //    await page.GetByLabel("Close").ClickAsync();
                //}
                //Dismiss Modal if present

                    bool isVisble = await TryWaitForLocatorVisible(page, page.Locator("#popup_1"));
                    //var modalPopup = page.Locator("#popup_1").WaitForAsync().;
                    if (isVisble)
                    {
                        await page.GetByLabel("Close").IsVisibleAsync();
                        await page.GetByLabel("Close").ClickAsync();
                    }

                    await page.GetByRole(AriaRole.Heading, new() { Name = "SummaryClient, Automation" }).ClickAsync();
                    await page.GetByRole(AriaRole.List).GetByText("Products").ClickAsync();
                    await page.GetByRole(AriaRole.Heading, new() { Name = "Products", Exact = true }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Add new product" }).ClickAsync();
                    await page.GetByRole(AriaRole.Heading, new() { Name = "FAIS Control details" }).ClickAsync();

                //});
            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }
        }

        /// <summary>
        /// Waits for a locator to become visible within a specified timeout
        /// </summary>
        /// <param name="page">The Playwright Page</param>
        /// <param name="locator">The locator to wait for</param>
        /// <param name="timeoutInSeconds">Maximum wait time in seconds</param>
        /// <returns>True if locator becomes visible, False if timeout occurs</returns>
        public static async Task<bool> TryWaitForLocatorVisible(
            IPage page,
            ILocator locator,
            float timeoutInSeconds = 10)
        {
            try
            {
                // Wait for the locator with a specific timeout
                await locator.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = timeoutInSeconds * 1000 // Convert to milliseconds
                });
                return true;
            }
            catch (TimeoutException)
            {
                // Locator did not become visible within the timeout
                return false;
            }
        }

    }
}