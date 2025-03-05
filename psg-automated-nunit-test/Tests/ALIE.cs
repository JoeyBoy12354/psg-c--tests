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
    [TestFixture, Order(12)]
    public sealed class ALIE : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;
        private readonly int _timeOutMs = 120_000;

        public ALIE() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }



        [Test]
        [Prod]
        public async Task OwnerExistingAddNew()
        {
            // arrange
            Name = "ALIE - Owner existing and Add new (ALIE)";

            Description = @"Add existing family member as Owner and ensure % saves​ Add new business and save ownership video to be based on DD-17745)";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            // act

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await page.GoToMyPracticeAsync(config, _otpService);

                    await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                       .ClickAsync(new() { Timeout = _timeOutMs });
                    await page.GetByText("Financial Adviser").ClickAsync();
                    await page.Locator("[id=\"__search\"]").ClickAsync();
                    await page.Locator("[id=\"__search\"]").FillAsync("Alie Test Client");
                    await page.Locator("[id=\"__search\"]").PressAsync("Enter");
                    await page.GetByText("Alie Test Client in Clients").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 Liabilities, Alie Test" }).Locator("path").ClickAsync();
                    await page.GetByPlaceholder("Find").ClickAsync();
                    await page.GetByRole(AriaRole.List).GetByText("Products").ClickAsync();
                    await page.GetByText("Balance Sheet").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 PSG Wealth - Living annuity" }).Locator("svg").First.ClickAsync();
                    await page.GetByLabel("Additional Owner").SelectOptionAsync(new[] { "1712346" });
                    await page.GetByLabel("Alie Test, Spouse").ClickAsync();
                    await page.GetByLabel("Alie Test, Spouse").FillAsync("25");
                    await page.GetByLabel("Alie Test, Spouse").PressAsync("Enter");
                    await page.GetByLabel("Liabilities, Alie Test Client").ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").FillAsync("75");
                    await page.GetByLabel("Liabilities, Alie Test Client").PressAsync("Enter");
                    await page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
                    await page.GetByText("Balance Sheet").ClickAsync();
                    await page.Locator("tr:nth-child(2) > .ui-table-cell-actions").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 PSG Wealth - Living annuity" }).Locator("path").First.ClickAsync();
                    await page.GetByLabel("Additional Owner").SelectOptionAsync(new[] { "1706037" });
                    await page.GetByLabel("Testautomation Association").ClickAsync();
                    await page.GetByLabel("Testautomation Association").FillAsync("25");
                    await page.GetByLabel("Liabilities, Alie Test Client").ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").FillAsync("50");
                    await page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
                    await page.GetByText("Balance Sheet").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 PSG Wealth - Living annuity" }).Locator("svg").First.ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Remove" }).Nth(1).ClickAsync();
                    await page.GetByText("Alie Test, Spouse", new() { Exact = true }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Remove" }).Nth(0).ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").ClickAsync();
                    await page.GetByLabel("Liabilities, Alie Test Client").FillAsync("100");
                    await page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
                    await page.GetByText("Balance Sheet").ClickAsync();
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