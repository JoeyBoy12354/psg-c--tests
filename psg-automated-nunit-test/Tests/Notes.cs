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
    [TestFixture, Order(4)]
    public sealed class Notes : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly int _timeOutMs = 3000;//120_000;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;

        private readonly string _idNrForNote = "8109220200084";//"9004226197088"; 

        private readonly string _incompleteNoteName = "Test Create Incomplete Note";


        public Notes() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }


        [Test]
        [Prod]
        public async Task Notes_CompleteNote()
        {
            //Arrange
            Name = "Notes - Complete Note";

            Description = "Log in Johan Website ( role Assistant) and then manage the first note and complete it";

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
                    await page.GetByText("Administrator").ClickAsync();
                    await page.GetByText("Assistant").ClickAsync();
                    await page.Locator("[id=\"__search\"]").ClickAsync();
                    await page.Locator("[id=\"__search\"]").FillAsync("8109220200084");
                    await page.GetByText("in Clients").ClickAsync();
                    await page.Locator("td").First.ClickAsync();
                    await page.GetByText("Notes / Tasks").ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).ClickAsync();
                    await page.GetByLabel("Owned by user").SelectOptionAsync(new[] { "1653694" });
                    await page.GetByLabel("Subject").ClickAsync();
                    await page.GetByLabel("Subject").FillAsync("Test Create Incomplete Note");
                    await page.GetByLabel("Interaction").SelectOptionAsync(new[] { "2" });
                    await page.GetByRole(AriaRole.Button, new() { Name = "Save new note", Exact = true }).ClickAsync();
                    
                    //await page.GetByRole(AriaRole.List).GetByText("Notes / Tasks").ClickAsync();
                    await page.GetByText("Notes / Tasks").ScrollIntoViewIfNeededAsync();
                    await page.GetByText("Notes / Tasks", new()).ClickAsync(new () { Force = true});
                    //await page.GetByText("Notes / Tasks").ClickAsync();
                    //await page.GetByRole(AriaRole.Row, new() { Name = "Email 1" , Exact = false}).Locator("path").First.ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Complete" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();
                    //await page.Locator("[id=\"__search_tasks\"]").ClickAsync();
                    //await page.Locator("[id=\"__search_tasks\"]").FillAsync("Test Create Incomplete Note");
                    //await page.GetByText("Test Create Incomplete Note in Subject", new() { Exact = true }).ClickAsync();
                    //await page.GetByRole(AriaRole.Row, new() { Name = "1 Brenner, Tasneem 8109220200084 Test Create Incomplete Note Email PSG Financial Services Ltd PSG Financial Services Psg Website, Johan Psg Website, Johan 1 2024-12-09 - None 2024-12-09 3" }).Locator("path").First.ClickAsync();
                    //await page.GetByRole(AriaRole.Button, new() { Name = "Complete" }).ClickAsync();
                    //await page.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();
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