using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Playwright;
using Polly;
using Psg.Common.Registrations.Polly.Policies;
using psg_automated_nunit_shared.Attributes;
using psg_automated_nunit_shared.Configurations;
using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Contracts.Base;
using psg_automated_nunit_shared.Managers;
using psg_automated_nunit_shared.PageExtensions;
using psg_automated_nunit_shared.Services;
using System.Reflection;


namespace psg_automated_nunit_test.Tests
{

    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!


    //[Parallelizable(ParallelScope.Fixtures)]
    [TestFixture, Order(7)]
    public sealed class Reports : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly int _timeOutMs = 120_000;
        private readonly IOtpService _otpService;

        public Reports() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }



        [Test]
        [Prod]
        public async Task Reports_CRS_related_field()
        {
            // arrange
            Name = "Report - with CRS related and product field";

            Description = "The user selected a CRS related field (Adviser under Entity column) together with a product field (Reason for cancellation under Product column)";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await page.GoToMyPracticeAsync(config, _otpService);

                    await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                                    .ClickAsync(new() { Timeout = _timeOutMs });

                    await page.GetByText("Assistant").ClickAsync();
                    await page.GetByText("Reports", new() { Exact = true }).First.ClickAsync();
                    await page.GetByText("Reports (Current)").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "Dynamic" }).Locator("path").ClickAsync();
                    await page.GetByLabel("Saved Selection").SelectOptionAsync(new[] { "50536" });
                    var download = await page.RunAndWaitForDownloadAsync(async () =>
                    {
                        await page.GetByRole(AriaRole.Button, new() { Name = "Download" }).ClickAsync();
                    });

                    string Location = Assembly.GetExecutingAssembly().Location;
                    var directoryPath = Path.Combine(Path.GetDirectoryName(Location)!, "Downloads");
                    Directory.CreateDirectory(directoryPath);

                    var DownloadPath = Path.Combine(directoryPath, download.SuggestedFilename);

                    // Get the downloaded file's path
                    await download.SaveAsAsync(DownloadPath);

                    // Wait for the download to complete and then delete the file
                    if (File.Exists(DownloadPath))
                    {
                        File.Delete(DownloadPath);
                    }
                });
            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }
        }

        [Test]
        [Prod]
        public async Task Dynamic_Report_CRS_Filters()
        {
            // arrange
            Name = "Dynamic report with CRS Filters as Administrator";

            Description = "When logged in as Administrator and you download a dynamic report that contains fields CRS and add filter CRS: Type then the report fails ";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            try
            {
                {
                    await page.GoToMyPracticeAsync(config, _otpService);

                    await page.GetByRole(AriaRole.Heading, new() { Name = $"{_testConfig.Name} {_testConfig.Surname} Logged in as" })
                                                                                                                                    .ClickAsync(new() { Timeout = _timeOutMs });
                    await page.GetByText("Reports", new() { Exact = true }).First.ClickAsync();
                    await page.GetByText("Reports (Beta)").ClickAsync();
                    await page.GetByRole(AriaRole.Row, new() { Name = "Dynamic" }).Locator("path").ClickAsync();
                    await page.GetByLabel("Saved Selection").SelectOptionAsync(new[] { "49747" });
                    await page.GetByRole(AriaRole.Button, new() { Name = "Download" }).ClickAsync();
                    Thread.Sleep(5000);
                    var otp = await _otpService.GetOtpDownloadAsync(page);
                    await page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your PIN" }).FillAsync(otp);

                    var download = await page.RunAndWaitForDownloadAsync(async () =>
                    {
                        await page.GetByRole(AriaRole.Button, new() { Name = "Download" }).Nth(1).ClickAsync();
                    }, new() { Timeout = _timeOutMs });
                };
            }
            finally
            {
                await StoreScreenShotAsync(page);
                await context.CloseAsync();
            }
        }
    }
}