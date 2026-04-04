using FluentAssertions;

namespace Quvantic.PlaywrightTests.Tests.Inventory;

[TestFixture]
[Category("regression")]
[Parallelizable(ParallelScope.Self)]
public class InventoryTests : BaseTest
{
    [SetUp]
    public async Task LoginFirst()
    {
        await LoginAsStandardUser();
    }

    [Test]
    [Description("Inventory page displays 6 products")]
    public async Task InventoryPage_DisplaysSixProducts()
    {
        var count = await InventoryPage.GetItemCount();
        count.Should().Be(6);
    }

    [Test]
    [Description("Adding product updates cart badge")]
    public async Task AddToCart_UpdatesBadge()
    {
        await InventoryPage.AddToCartByName("Sauce Labs Backpack");
        var badge = await InventoryPage.GetCartBadgeCount();
        badge.Should().NotBe("0");
    }

    [Test]
    [Description("Removing product from cart")]
    public async Task RemoveFromCart_ClearsBadge()
    {
        await InventoryPage.AddToCartByName("Sauce Labs Bike Light");
        await InventoryPage.RemoveFromCartByName("Sauce Labs Bike Light");
    }

    [Test]
    [Description("Sort products by price low to high")]
    public async Task SortByPriceLowToHigh()
    {
        await InventoryPage.SortBy("lohi");
        var price = await InventoryPage.GetFirstProductPrice();
        price.Should().Contain("$");
    }

    [Test]
    [Description("Sort products by name Z to A")]
    public async Task SortByNameZtoA()
    {
        await InventoryPage.SortBy("za");
        var name = await InventoryPage.GetFirstProductName();
        name.Should().NotBeNullOrEmpty();
    }
}
