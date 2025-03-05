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
    [TestFixture, Order(10)]
    public sealed class Advice : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly int _timeOutMs = 120_000;
        private readonly IOtpService _otpService;

        public Advice() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }


        [Test]
        [Prod]
        public async Task Advice_Cashflow_Allocate_EW_Product()
        {
            // arrange
            Name = "Advice - Cashflow Allocate EW Product";

            Description = "Allocate EW Product";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            // act

            try
            {
                await page.GoToMyPracticeAsync(config, _otpService);

                await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                       .ClickAsync(new() { Timeout = _timeOutMs });
                await page.GetByText("Financial Adviser").ClickAsync();
                await page.GetByPlaceholder("Find").Nth(0).ClickAsync();
                await page.GetByPlaceholder("Find").Nth(0).FillAsync("0103045694082");
                await page.GetByText("0103045694082 in Clients").ClickAsync();

                await page.GetByRole(AriaRole.Row, new() { Name = "1 Client, Automation" }).Locator("svg").ClickAsync();

                bool isVisble = await TryWaitForLocatorVisible(page, page.Locator("#popup_1"));
                if (isVisble)
                {
                    await page.GetByRole(AriaRole.Button, new() { Name = "Close" }).IsVisibleAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync();
                }

                await page.GetByRole(AriaRole.List).GetByText("Workflows").ClickAsync(new() { Timeout = 300 });
                await page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).First.ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Next step" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Next step" }).ClickAsync();
                await page.Locator("a").Filter(new() { HasText = "Investment plan - Cashflow" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).First.ClickAsync();
                await page.GetByLabel("Scenario name").ClickAsync();
                await page.GetByLabel("Scenario name").FillAsync("AP Cashflow - Allocate EW product\"");
                await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

                //Wait here for long enough
                await page.WaitForTimeoutAsync(17000);
                await page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Row, new() { Name = "Objective" }).GetByRole(AriaRole.Textbox).ClickAsync();
                await page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Row, new() { Name = "Objective" }).GetByRole(AriaRole.Textbox).FillAsync("AP Cashflow - Allocate EW product");
                await page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Row, new() { Name = "Objective AP Cashflow -" }).GetByRole(AriaRole.Textbox).ClickAsync();
                await page.Locator("iframe").ContentFrame.GetByText("Income provisions and").ClickAsync();
                await page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Row, new() { Name = "+ Incl? Description Owner Available when? Current value Growth % Monthly" }).GetByRole(AriaRole.Button).ClickAsync();
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(3) > .width300px").ClickAsync();
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(3) > .width300px").FillAsync("vip");
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(6) > .width80px").ClickAsync();
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(6) > .width80px").PressAsync("ArrowRight");
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(6) > .width80px").FillAsync("1000");
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(8) > .width60px").ClickAsync();
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(8) > .width60px").PressAsync("ArrowRight");
                await page.Locator("iframe").ContentFrame.Locator("tr:nth-child(4) > td:nth-child(8) > .width60px").FillAsync("150");
                await page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

                ////Scroll sync button into view
                await page.GetByText("Sync scenario result to myPractice").ScrollIntoViewIfNeededAsync();
                await page.GetByRole(AriaRole.Button).GetByText("Sync scenario result to myPractice").ClickAsync(new() { Timeout = 3000 });

                await page.WaitForTimeoutAsync(17000);

                await page.GetByLabel("Close").ClickAsync();

                await page.Locator("a").Filter(new() { HasText = "Investment plan - Allocations" }).ClickAsync();
                await page.GetByRole(AriaRole.Row, new() { Name = "vip New Plan (Product Total)" }).Locator("path").First.ClickAsync();
                await page.GetByLabel("Product", new() { Exact = true }).ClickAsync();
                await page.Locator("#pro_id").SelectOptionAsync(new[] { "76" });

                await page.WaitForTimeoutAsync(600);

                await page.Locator("#prc_id").SelectOptionAsync(new[] { "2" });

                await page.WaitForTimeoutAsync(3000);
                await page.GetByRole(AriaRole.Row, new() { Name = "Investments Collective Investments (Local) PSG Wealth Voluntary Investment" }).Locator("span").WaitForAsync( new() { Timeout = 5000});
                await page.GetByRole(AriaRole.Row, new() { Name = "Investments Collective Investments (Local) PSG Wealth Voluntary Investment" }).Locator("span").ClickAsync();
                await page.GetByPlaceholder("-- Not Selected --").ClickAsync();
                await page.GetByRole(AriaRole.Option, new() { Name = "Salary" }).ClickAsync();
                await page.Locator("ul").Filter(new() { HasText = "×Salary" }).ClickAsync();

                await page.GetByLabel("Advice type").SelectOptionAsync(new[] { "1" });
                await page.GetByLabel("Manage type").SelectOptionAsync(new[] { "1" });
                await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

                await page.GetByLabel("Close").ClickAsync();
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