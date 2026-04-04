using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using Quvantic.PlaywrightTests.Pages;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Tests;

public class BaseTest : PageTest
{
    protected LoginPage LoginPage = null!;
    protected InventoryPage InventoryPage = null!;
    protected CartPage CartPage = null!;
    protected CheckoutPage CheckoutPage = null!;

    [SetUp]
    public void InitPages()
    {
        LoginPage = new LoginPage(Page);
        InventoryPage = new InventoryPage(Page);
        CartPage = new CartPage(Page);
        CheckoutPage = new CheckoutPage(Page);
    }

    protected async Task LoginAsStandardUser()
    {
        await LoginPage.Navigate();
        await LoginPage.Login(TestConfig.StandardUser, TestConfig.Password);
        await InventoryPage.WaitForLoad();
    }
}
