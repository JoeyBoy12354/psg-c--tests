using Microsoft.Playwright;

namespace psg_automated_nunit_test.Extensions
{
    public static class IPageExtensions
    {


        /// <summary>
        /// Checks for the special instructions popup and closes it, if it's there.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task CheckSpecialInstructionsAsync(this IPage page)
        {
            try
            {
                await page.GetByRole(AriaRole.Heading, new() { Name = "Special" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();
            }
            catch
            {
                // do nothing
            }
        }
    }
}
