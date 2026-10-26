using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public sealed class LoginPage(IPage page) : BasePage(page)
{
    private ILocator UsernameInput => ByTestId("username");
    private ILocator PasswordInput => ByTestId("password");
    private ILocator LoginButton => ByTestId("login-button");

    public ILocator ErrorMessage => ByTestId("error");

    public async Task GotoAsync() => await Page.GotoAsync("/");

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
