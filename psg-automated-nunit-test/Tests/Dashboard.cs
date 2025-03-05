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
    [TestFixture, Order(2)]
    public sealed class Dashboard : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;
        private readonly int _timeOutMs = 120_000;

        public Dashboard() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();

            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }
   

        [Test]
        [Prod]
        public async Task CheckMis()
        {
            // arrange
            Name = "MIS - Dashboard visible";

            Description = $@"{Environment.NewLine}Login to myPractice via any of the routes (myPSG/myBase/Alpha) using OTP(MFA)
                             {Environment.NewLine}Then switch roles to Financial Planner
                             {Environment.NewLine}Click on Fleur de lis(1)
                             {Environment.NewLine}Click on MIS(2)
                             {Environment.NewLine}Ensure that graphs are displaying(should be noted that our adviser does not have actual data for all categories) ";

            // MIS only works from OTP!

            context = await PlaywrightManager.GetNewBrowserContextAsync();

            var page = await context.NewPageAsync();
            var result = await page.LoginMyPsgAsync(_testConfig, _otpService);

            if(!string.IsNullOrWhiteSpace(result))
                Assert.Fail(result);

            // act

            try
            {
                // Enter myPractice
                await page.GoToMyPracticeByClickAsync();

                await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                                         .ClickAsync(new() { Timeout = _timeOutMs });

                await page.GetByText("Financial Adviser").ClickAsync();
                await page.Locator(".banner-logo").ClickAsync();
                await page.GetByText("MIS", new() { Exact = true }).ClickAsync();
                await page.FrameLocator("iframe").GetByText("Client iComply").ClickAsync();
                await page.FrameLocator("iframe").Locator("app-teamregionrevenue").GetByText("Clients Reviewed").ClickAsync();
                await page.FrameLocator("iframe").GetByText("CPD Hours", new() { Exact = true }).ClickAsync();


            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }

        }


      

    }
}