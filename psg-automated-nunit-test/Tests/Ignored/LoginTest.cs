using psg_automated_nunit_shared.Contracts;
using psg_automated_nunit_shared.Contracts.Base;
using psg_automated_nunit_shared.Managers;

namespace psg_automated_nunit_test.Tests.Ignored
{

    // DO NOT ADD ANY PSG PACKAGES!!
    // JENKINS CANNOT CONNECT TO THE PSG PACKAGE SERVER TO DOWNLOAD THEM !!!!


    [Parallelizable(ParallelScope.Fixtures)]
    [TestFixture]
    public sealed class LoginTest : PsgTestBase
    {
        public LoginTest() : base()
        {
        }



        [Test]
        [Ignore("Just used as an example")]
        public async Task Login_myPractice()
        {
            // arrange
            Description = "Tests a Login directly into myPractice.";

            if (!PlaywrightManager.IsLoggedIn)
                Assert.Fail("User no logged in");
        }



    }
}