using FluentAssertions;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Pages;
using Quvantic.PlaywrightTests.Utils;
using static Microsoft.Playwright.Assertions;

namespace Quvantic.PlaywrightTests.Tests.UI.Inventory;

[TestFixture]
public sealed class InventoryTests : UiTestBase
{
    [SetUp]
    public async Task LoginAsync() => await LoginAsStandardUserAsync();

    [Test, Category(TestCategories.Smoke)]
    public async Task Inventory_ShowsSixProducts()
    {
        await Expect(InventoryPage.Items).ToHaveCountAsync(6);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task AddToCart_ShowsBadgeCount()
    {
        await InventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await InventoryPage.AddToCartAsync("Sauce Labs Bike Light");

        await Expect(InventoryPage.CartBadge).ToHaveTextAsync("2");
    }

    [Test, Category(TestCategories.Regression)]
    public async Task RemoveFromCart_HidesBadge()
    {
        await InventoryPage.AddToCartAsync("Sauce Labs Bike Light");
        await Expect(InventoryPage.CartBadge).ToHaveTextAsync("1");

        await InventoryPage.RemoveFromCartAsync("Sauce Labs Bike Light");

        await Expect(InventoryPage.CartBadge).ToBeHiddenAsync();
    }

    [Test, Category(TestCategories.Regression)]
    [TestCase(SortOption.NameAscending)]
    [TestCase(SortOption.NameDescending)]
    public async Task SortByName_OrdersProducts(SortOption option)
    {
        await InventoryPage.SortByAsync(option);

        var names = await InventoryPage.GetProductNamesAsync();

        names.Should().HaveCount(6);
        if (option == SortOption.NameAscending)
            names.Should().BeInAscendingOrder(StringComparer.Ordinal);
        else
            names.Should().BeInDescendingOrder(StringComparer.Ordinal);
    }

    [Test, Category(TestCategories.Regression)]
    [TestCase(SortOption.PriceAscending)]
    [TestCase(SortOption.PriceDescending)]
    public async Task SortByPrice_OrdersProducts(SortOption option)
    {
        await InventoryPage.SortByAsync(option);

        var prices = await InventoryPage.GetProductPricesAsync();

        prices.Should().HaveCount(6);
        if (option == SortOption.PriceAscending)
            prices.Should().BeInAscendingOrder();
        else
            prices.Should().BeInDescendingOrder();
    }
}
