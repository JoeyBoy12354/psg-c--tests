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
    [TestFixture, Order(3)]
    public sealed class Icomply : PsgTestBase
    {

        private readonly IAsyncPolicy _retryPolicy;
        private readonly TestConfiguration _testConfig;
        private readonly IOtpService _otpService;

        private readonly string _idBankUser = "6509058674080";
        private readonly string _nameTestUser = "Johan Psg Website";


        public Icomply() : base()
        {
            _retryPolicy = PollyPolicies.GetRetryPolicyAsync();
            _testConfig = DependencyManager.GetRequiredService<TestConfiguration>();
            _otpService = DependencyManager.GetService<IOtpService>()!;
        }


        [Test]
        [Prod]
        public async Task Icomply_ClientInformationSheet()
        {
            //Arrange
            Name = "Icomply - Client Information Sheet";

            Description = "Client with multiple bank accounts on profile, when iComply is generated that included the client information sheet then it would not download. DD-18922 ";

            context = await PlaywrightManager.LoginMyPracticeAndGetContextAsync();

            var page = await context.NewPageAsync();

            // act

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await page.GoToMyPracticeAsync(config, _otpService);

                    //Find test user
                    var find = page.GetByPlaceholder("Find").Nth(0);
                    await find.ClickAsync();
                    await find.FillAsync(_idBankUser);
                    await page.GetByText($"{_idBankUser} in Clients").ClickAsync();

                    //Manage user
                    await page.GetByRole(AriaRole.Row).Filter(new() { HasText = _idBankUser }).Locator("path").First.ClickAsync();

                    //iComply, get client information sheet
                    await page.GetByRole(AriaRole.List).GetByText("iComply").ClickAsync();
                    await page.GetByRole(AriaRole.Row).Filter(new() { HasText = _nameTestUser }).Locator("path").First.ClickAsync();
                    await page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Client information sheet" }).Locator("path").Nth(1).ClickAsync();

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