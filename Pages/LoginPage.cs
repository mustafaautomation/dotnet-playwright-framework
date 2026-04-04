using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public class LoginPage : BasePage
{
    private ILocator UsernameInput => Page.Locator("[data-test='username']");
    private ILocator PasswordInput => Page.Locator("[data-test='password']");
    private ILocator LoginButton => Page.Locator("[data-test='login-button']");
    private ILocator ErrorMessage => Page.Locator("[data-test='error']");

    public LoginPage(IPage page) : base(page) { }

    public async Task Navigate()
    {
        await Page.GotoAsync(Utils.TestConfig.BaseUrl);
        await LoginButton.WaitForAsync();
    }

    public async Task Login(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetErrorMessage()
    {
        await ErrorMessage.WaitForAsync();
        return await ErrorMessage.TextContentAsync() ?? string.Empty;
    }

    public async Task<bool> IsErrorVisible()
    {
        return await ErrorMessage.IsVisibleAsync();
    }
}
