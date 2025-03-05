using Microsoft.Playwright;

namespace psg_automated_nunit_shared.Contracts
{
    public interface IOtpService
    {
        Task<string> GetOtpAsync(IPage page, string phoneNumber = "", int maxRetries = 10);
        Task<string> GetOtpDownloadAsync(IPage page, string phoneNumber = "", int maxRetries = 10);

    }
}