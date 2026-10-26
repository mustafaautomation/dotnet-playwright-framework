using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public sealed class CartPage(IPage page) : BasePage(page)
{
    private ILocator CheckoutButton => ByTestId("checkout");
    private ILocator ContinueShoppingButton => ByTestId("continue-shopping");

    public ILocator Items => Page.Locator(".cart_item");
    public ILocator ItemNames => Page.Locator(".cart_item .inventory_item_name");

    public async Task CheckoutAsync() => await CheckoutButton.ClickAsync();

    public async Task ContinueShoppingAsync() => await ContinueShoppingButton.ClickAsync();
}
