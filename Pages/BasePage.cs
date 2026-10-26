using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

/// <summary>
/// Page objects expose user-level actions and the locators tests need for web-first
/// assertions (<c>Expect(locator)</c>). They do not contain assertions themselves.
/// </summary>
public abstract class BasePage(IPage page)
{
    protected IPage Page { get; } = page;

    protected ILocator ByTestId(string testId) => Page.Locator($"[data-test='{testId}']");
}
