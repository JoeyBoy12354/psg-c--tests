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
    [TestFixture, Order(1)]
    public sealed class Communications : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;
        private readonly int _timeOutMs = 120_000;

        public Communications() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }



        [Test]
        [Prod]
        public async Task CheckMailBox()
        {
            // arrange
            Name = "Communications - Check mailbox";

            Description = "any actions taken on outlook, to be read from the outlook server and be displayed on myPractice with the relevant icons. " +
                " The functionality of setting the relevant flags (replied/forwarded) for each email on the outlook server, based on actions done on myPractice mailbox " +
                "video to be based on CRMMAINT-377408)";


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
                    await page.GetByText("Communication").First.ClickAsync();
                    await page.GetByText("myMailbox").ClickAsync();
                    await page.Locator("tr:has-text('|PSG|')").First.Locator("svg").First.ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "More actions" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Forward" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Back", Exact = true }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "More actions" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Reply all" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Back", Exact = true }).ClickAsync();

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