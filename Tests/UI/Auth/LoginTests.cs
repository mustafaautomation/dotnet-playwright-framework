using System.Text.RegularExpressions;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Config;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Utils;
using static Microsoft.Playwright.Assertions;

namespace Quvantic.PlaywrightTests.Tests.UI.Auth;

[TestFixture]
public sealed class LoginTests : UiTestBase
{
    [SetUp]
    public async Task OpenLoginPageAsync() => await LoginPage.GotoAsync();

    [Test, Category(TestCategories.Smoke)]
    public async Task ValidCredentials_NavigateToInventory()
    {
        await LoginPage.LoginAsync(Settings.Username, Settings.Password);

        await Expect(Page).ToHaveURLAsync(new Regex("/inventory\\.html$"));
        await Expect(InventoryPage.Items.First).ToBeVisibleAsync();
    }

    [Test, Category(TestCategories.Regression)]
    [TestCaseSource(nameof(InvalidLogins))]
    public async Task InvalidLogin_ShowsError(string username, string password, string expectedError)
    {
        await LoginPage.LoginAsync(username, password);

        await Expect(LoginPage.ErrorMessage).ToContainTextAsync(expectedError);
        await Expect(Page).Not.ToHaveURLAsync(new Regex("inventory"));
    }

    private static IEnumerable<TestCaseData> InvalidLogins()
    {
        var password = TestSettings.Current.Ui.Password;

        yield return new TestCaseData("locked_out_user", password, "Sorry, this user has been locked out")
            .SetArgDisplayNames("locked out user");
        yield return new TestCaseData("", password, "Username is required")
            .SetArgDisplayNames("empty username");
        yield return new TestCaseData("standard_user", "", "Password is required")
            .SetArgDisplayNames("empty password");
        yield return new TestCaseData("unknown_user", "wrong_password", "do not match any user")
            .SetArgDisplayNames("wrong credentials");
    }
}
