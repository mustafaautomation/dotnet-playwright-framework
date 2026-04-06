using NUnit.Framework;
using FluentAssertions;

namespace Quvantic.PlaywrightTests.Tests.Cart;

[TestFixture]
[Category("regression")]
[Parallelizable(ParallelScope.Self)]
public class CartTests : BaseTest
{
    [SetUp]
    public async Task LoginFirst()
    {
        await LoginAsStandardUser();
    }

    [Test]
    [Description("Cart shows added items")]
    public async Task Cart_DisplaysAddedItems()
    {
        await InventoryPage.AddToCartByName("Sauce Labs Backpack");
        await InventoryPage.AddToCartByName("Sauce Labs Bike Light");
        await InventoryPage.OpenCart();

        var count = await CartPage.GetItemCount();
        count.Should().Be(2);
    }

    [Test]
    [Description("Cart displays correct product")]
    public async Task Cart_ContainsCorrectProduct()
    {
        await InventoryPage.AddToCartByName("Sauce Labs Bolt T-Shirt");
        await InventoryPage.OpenCart();

        var contains = await CartPage.ContainsProduct("Sauce Labs Bolt T-Shirt");
        contains.Should().BeTrue();
    }

    [Test]
    [Description("Empty cart shows zero items")]
    public async Task EmptyCart_ShowsZeroItems()
    {
        await InventoryPage.OpenCart();

        var count = await CartPage.GetItemCount();
        count.Should().Be(0);
    }

    [Test]
    [Description("Checkout button navigates to step one")]
    public async Task Checkout_NavigatesToStepOne()
    {
        await InventoryPage.AddToCartByName("Sauce Labs Backpack");
        await InventoryPage.OpenCart();
        await CartPage.Checkout();

        var url = Page.Url;
        url.Should().Contain("checkout-step-one");
    }
}
