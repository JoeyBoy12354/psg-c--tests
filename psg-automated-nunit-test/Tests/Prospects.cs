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
    [TestFixture, Order(6)]
    public sealed class Prospects : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly int _timeOutMs = 120_000;
        private readonly IOtpService _otpService;

        private const string _idSigogo = "8708076166081";
        private const string _nameSigogo = "Sigogo";

        private const string _prospectSigogo = "Estate Late Sigogo Sigogo";



        public Prospects() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }
       

        [Test]
        [Prod]
        public async Task Prospects_Saving_Not_Blank()
        {
            // arrange
            Name = "Prospect - Saving prospect";

            Description = "When adding a prospect and click on add then you are not taken to prospects screen - blank (DDM-505).";

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

                    // first delete him if he's there already
                    await page.GetByText("Clients", new() { Exact = true }).First.ClickAsync();
                    await page.GetByText("Prospects", new() { Exact = true }).ClickAsync();

                    // getbthe second find box on the screen
                    var find = page.GetByPlaceholder("Find").Nth(1);
                    await find.ClickAsync();
                    await find.FillAsync(_idSigogo);
                    await find.PressAsync("Enter", new() { Timeout = _timeOutMs });

                    // now check if the prospect is there
                    // the second item Nth(1) is the delete button
                    var prospectbyId = page.GetByRole(AriaRole.Row, new() { Name = $"{_prospectSigogo}" })
                    .Locator("path")
                    .Nth(1);

                    var exists = await page.IsFoundInListAsync(prospectbyId);

                    if(exists)
                    {
                        //await prospect.ClickAsync();
                        await prospectbyId.ClickAsync();
                        await page.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();
                    }

                    // try to Add/Link the prospect
                    await page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).ClickAsync();
                    await page.GetByLabel("ID Number").ClickAsync();
                    await page.GetByLabel("ID Number").FillAsync(_idSigogo);
                    await page.GetByRole(AriaRole.Button, new() { Name = "Next step" }).ClickAsync();

                    await page.GetByRole(AriaRole.Heading, new() { Name = "Referral details" }).ClickAsync();

                    // first check if the link button is avaiable
                    var addAsProspect = page.GetByRole(AriaRole.Button, new() { Name = "Add as prospect" });

                    if(await addAsProspect.IsEnabledAsync())
                    {
                        await addAsProspect.ClickAsync();
                    }
                    else
                    {
                        // no link, must be added first                    
                     
                    
                        await page.GetByLabel("Initials").ClickAsync();
                        await page.GetByLabel("Initials").FillAsync("K");
                        await page.GetByLabel("First names").ClickAsync();
                        await page.GetByLabel("First names").FillAsync($"{_prospectSigogo}"); //Updated Name
                        await page.GetByLabel("Surname").ClickAsync();
                        await page.GetByLabel("Surname").FillAsync($"{_prospectSigogo}"); //Updated Name
                        await page.GetByRole(AriaRole.Button, new() { Name = "Finish" }).ClickAsync();
                    }

                await page.GetByRole(AriaRole.Heading, new() { Name = "SummaryEstate Late Sigogo" }).ClickAsync();


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