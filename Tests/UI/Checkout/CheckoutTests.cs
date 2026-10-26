using FluentAssertions;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Utils;
using static Microsoft.Playwright.Assertions;

namespace Quvantic.PlaywrightTests.Tests.UI.Checkout;

[TestFixture]
public sealed class CheckoutTests : UiTestBase
{
    [SetUp]
    public async Task StartCheckoutWithBackpackAsync()
    {
        await LoginAsStandardUserAsync();
        await InventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await InventoryPage.OpenCartAsync();
        await CartPage.CheckoutAsync();
    }

    [Test, Category(TestCategories.Smoke)]
    public async Task CompleteCheckout_ShowsConfirmation()
    {
        await CheckoutPage.FillCustomerInfoAsync("Test", "User", "12345");
        await CheckoutPage.ContinueAsync();

        var summary = await CheckoutPage.GetOrderSummaryAsync();
        summary.ItemTotal.Should().Be(29.99m);
        summary.Total.Should().Be(summary.ItemTotal + summary.Tax);

        await CheckoutPage.FinishAsync();

        await Expect(CheckoutPage.CompleteHeader).ToHaveTextAsync("Thank you for your order!");
    }

    [Test, Category(TestCategories.Regression)]
    [TestCase("", "User", "12345", "First Name is required")]
    [TestCase("Test", "", "12345", "Last Name is required")]
    [TestCase("Test", "User", "", "Postal Code is required")]
    public async Task MissingCustomerInfo_ShowsValidationError(
        string firstName, string lastName, string postalCode, string expectedError)
    {
        await CheckoutPage.FillCustomerInfoAsync(firstName, lastName, postalCode);
        await CheckoutPage.ContinueAsync();

        await Expect(CheckoutPage.ErrorMessage).ToContainTextAsync(expectedError);
    }
}
