using FluentAssertions;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Tests.Auth;

[TestFixture]
[Category("smoke")]
[Parallelizable(ParallelScope.Self)]
public class LoginTests : BaseTest
{
    [SetUp]
    public async Task NavigateToLogin()
    {
        await LoginPage.Navigate();
    }

    [Test]
    [Description("Standard user can log in successfully")]
    public async Task ValidLogin_NavigatesToInventory()
    {
        await LoginPage.Login(TestConfig.StandardUser, TestConfig.Password);
        var url = await LoginPage.GetCurrentUrl();
        url.Should().Contain("inventory");
    }

    [Test]
    [Description("Locked out user sees error message")]
    public async Task LockedUser_ShowsError()
    {
        await LoginPage.Login("locked_out_user", TestConfig.Password);
        var error = await LoginPage.GetErrorMessage();
        error.Should().Contain("locked out");
    }

    [Test]
    [Description("Empty username shows validation error")]
    public async Task EmptyUsername_ShowsError()
    {
        await LoginPage.Login("", TestConfig.Password);
        var error = await LoginPage.GetErrorMessage();
        error.Should().Contain("Username is required");
    }

    [Test]
    [Description("Empty password shows validation error")]
    public async Task EmptyPassword_ShowsError()
    {
        await LoginPage.Login(TestConfig.StandardUser, "");
        var error = await LoginPage.GetErrorMessage();
        error.Should().Contain("Password is required");
    }

    [Test]
    [Description("Invalid credentials show error")]
    public async Task InvalidCredentials_ShowsError()
    {
        await LoginPage.Login("bad_user", "bad_pass");
        var error = await LoginPage.GetErrorMessage();
        error.Should().Contain("do not match");
    }
}
