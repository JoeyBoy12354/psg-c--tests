using Microsoft.Playwright;
using Polly;
using Psg.Common.Registrations.Polly.Policies;
using psg_automated_nunit_shared.Attributes;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Contracts.Base;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.PageExtensions;
using psg_automated_nunit_test.Extensions;

namespace psg_automated_nunit_test.Tests
{

    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!

    //[Parallelizable(ParallelScope.Fixtures)]
    [TestFixture, Order(13)]
    public sealed class Crm : PsgTestBase
    {
        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;
        private readonly int _timeOutMs = 120_000;

        public Crm() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }


        [Test]
        [Prod]
        public async Task Crm_ChangesDontTriggerUpdates()
        {
            //Arrange
            Name = "CRM Tab - changes in specific areas not to trigger updates in other sections on CRM Tab";

            Description = "When making changes in one specific section on the CRM tab and saving those changes should not prompt user to update details in another section.";

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
                    await Task.Delay(3000);
                    await page.Locator("[id=\"__search\"]").ClickAsync();
                    await page.Locator("[id=\"__search\"]").FillAsync("0103045694082");
                    await Task.Delay(3000);
                    await page.GetByText("in Clients").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "1 Client, Automation" }).Locator("svg").ClickAsync();

                    await page.CheckSpecialInstructionsAsync();

                    //await page.GetByLabel("Close").ClickAsync();          // popup close logic must come here AND it must account for if there is NO popup!

                    await page.GetByText("CRM").ClickAsync();
                    await page.GetByLabel("Industry").SelectOptionAsync(new[] { "115" });
                    await page.GetByRole(AriaRole.Button, new() { Name = "Save changes" }).ClickAsync();
                    await page.GetByLabel("Industry").SelectOptionAsync(new[] { "89" });
                    await page.GetByRole(AriaRole.Button, new() { Name = "Save changes" }).ClickAsync();
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
