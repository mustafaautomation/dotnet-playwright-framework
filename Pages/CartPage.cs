using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public class CartPage : BasePage
{
    private ILocator CartItems => Page.Locator(".cart_item");
    private ILocator CheckoutButton => Page.Locator("[data-test='checkout']");
    private ILocator ContinueShopping => Page.Locator("[data-test='continue-shopping']");

    public CartPage(IPage page) : base(page) { }

    public async Task<int> GetItemCount()
    {
        return await CartItems.CountAsync();
    }

    public async Task<bool> ContainsProduct(string productName)
    {
        var text = await Page.Locator(".cart_list").TextContentAsync() ?? string.Empty;
        return text.Contains(productName);
    }

    public async Task Checkout()
    {
        await CheckoutButton.ClickAsync();
        await Page.WaitForURLAsync("**/checkout-step-one*");
    }

    public async Task ContinueShopping_Click()
    {
        await ContinueShopping.ClickAsync();
        await Page.WaitForURLAsync("**/inventory*");
    }
}
