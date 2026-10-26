using System.Text.RegularExpressions;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Utils;
using static Microsoft.Playwright.Assertions;

namespace Quvantic.PlaywrightTests.Tests.UI.Cart;

[TestFixture]
public sealed class CartTests : UiTestBase
{
    [SetUp]
    public async Task LoginAsync() => await LoginAsStandardUserAsync();

    [Test, Category(TestCategories.Smoke)]
    public async Task Cart_ListsAddedProducts()
    {
        await InventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await InventoryPage.AddToCartAsync("Sauce Labs Bolt T-Shirt");
        await InventoryPage.OpenCartAsync();

        await Expect(CartPage.ItemNames).ToHaveTextAsync(["Sauce Labs Backpack", "Sauce Labs Bolt T-Shirt"]);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task EmptyCart_HasNoItems()
    {
        await InventoryPage.OpenCartAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/cart\\.html$"));
        await Expect(CartPage.Items).ToHaveCountAsync(0);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task ContinueShopping_ReturnsToInventoryAndKeepsCart()
    {
        await InventoryPage.AddToCartAsync("Sauce Labs Onesie");
        await InventoryPage.OpenCartAsync();

        await CartPage.ContinueShoppingAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/inventory\\.html$"));
        await Expect(InventoryPage.CartBadge).ToHaveTextAsync("1");
    }

    [Test, Category(TestCategories.Regression)]
    public async Task Checkout_OpensCustomerInfoStep()
    {
        await InventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await InventoryPage.OpenCartAsync();

        await CartPage.CheckoutAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-step-one\\.html$"));
    }
}
