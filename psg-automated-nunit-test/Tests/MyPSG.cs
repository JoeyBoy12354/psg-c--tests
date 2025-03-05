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
    [TestFixture, Order(11)]
    public sealed class MyPSG : PsgTestBase
    {
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;
        private readonly int _timeOutMs = 120_000;

        public MyPSG() : base()
        {
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }   

        [Test]
        [Prod]
        public async Task SubmitSupportQuery()
        {
            // arrange
            Name = "myPSG - Submit support query";

            Description = "To be able to submit a successful myPSG Support query";

            // MIS only works from OTP!

            context = await PlaywrightManager.GetNewBrowserContextAsync();

            var page = await context.NewPageAsync();
            var result = await page.LoginMyPsgAsync(_testConfig, _otpService);

            if(!string.IsNullOrWhiteSpace(result))
                Assert.Fail(result);

            // act

            try
            {
                await page.GetByRole(AriaRole.Link, new() { Name = "Provide feedback" }).ClickAsync();
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("SummaryRequired").ClickAsync();
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Description").ClickAsync();
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Description").FillAsync("Testing the myPSG Support query function");
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Name").ClickAsync();
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Name").FillAsync("Johan Website");
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Email").ClickAsync();
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Email").FillAsync("distribution.automation@psg.co.za");
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByLabel("Include data about your").CheckAsync();
                await page.Locator("iframe[title=\"Provide feedback\"]").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }

        }


      

    }
}