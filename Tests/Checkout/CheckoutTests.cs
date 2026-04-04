using FluentAssertions;

namespace Quvantic.PlaywrightTests.Tests.Checkout;

[TestFixture]
[Category("regression")]
[Parallelizable(ParallelScope.Self)]
public class CheckoutTests : BaseTest
{
    [SetUp]
    public async Task LoginAndAddProduct()
    {
        await LoginAsStandardUser();
        await InventoryPage.AddToCartByName("Sauce Labs Backpack");
    }

    [Test]
    [Description("Complete checkout with single item")]
    public async Task CompleteCheckout_SingleItem()
    {
        await InventoryPage.OpenCart();
        var hasProduct = await CartPage.ContainsProduct("Sauce Labs Backpack");
        hasProduct.Should().BeTrue();

        await CartPage.Checkout();
        await CheckoutPage.FillInfo("Test", "User", "12345");
        await CheckoutPage.Continue();

        var total = await CheckoutPage.GetTotal();
        total.Should().Contain("$");

        await CheckoutPage.Finish();
        var complete = await CheckoutPage.IsOrderComplete();
        complete.Should().BeTrue();
    }

    [Test]
    [Description("Checkout fails without first name")]
    public async Task Checkout_FailsWithoutFirstName()
    {
        await InventoryPage.OpenCart();
        await CartPage.Checkout();
        await CheckoutPage.FillInfo("", "User", "12345");
        await CheckoutPage.Continue();

        var error = await CheckoutPage.GetErrorMessage();
        error.Should().Contain("First Name is required");
    }

    [Test]
    [Description("Checkout fails without postal code")]
    public async Task Checkout_FailsWithoutPostalCode()
    {
        await InventoryPage.OpenCart();
        await CartPage.Checkout();
        await CheckoutPage.FillInfo("Test", "User", "");
        await CheckoutPage.Continue();

        var error = await CheckoutPage.GetErrorMessage();
        error.Should().Contain("Postal Code is required");
    }
}
