using Microsoft.Playwright;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
        Page.SetDefaultTimeout(TestConfig.DefaultTimeout);
    }

    public async Task<string> GetCurrentUrl() => Page.Url;

    public async Task WaitForUrl(string urlFragment)
    {
        await Page.WaitForURLAsync($"**/{urlFragment}**");
    }

    public async Task<byte[]> TakeScreenshot(string? name = null)
    {
        return await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            FullPage = true,
            Path = name != null ? $"screenshots/{name}.png" : null
        });
    }
}
