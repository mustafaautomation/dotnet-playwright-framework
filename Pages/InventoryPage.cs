using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public class InventoryPage : BasePage
{
    private ILocator InventoryItems => Page.Locator(".inventory_item");
    private ILocator CartBadge => Page.Locator(".shopping_cart_badge");
    private ILocator CartLink => Page.Locator(".shopping_cart_link");
    private ILocator SortDropdown => Page.Locator("[data-test='product-sort-container']");
    private ILocator FirstItemName => Page.Locator(".inventory_item_name").First;
    private ILocator FirstItemPrice => Page.Locator(".inventory_item_price").First;

    public InventoryPage(IPage page) : base(page) { }

    public async Task WaitForLoad()
    {
        await InventoryItems.First.WaitForAsync();
    }

    public async Task<int> GetItemCount()
    {
        return await InventoryItems.CountAsync();
    }

    public async Task AddToCartByName(string productName)
    {
        var buttonId = $"add-to-cart-{productName.ToLower().Replace(" ", "-")}";
        await Page.Locator($"[data-test='{buttonId}']").ClickAsync();
    }

    public async Task RemoveFromCartByName(string productName)
    {
        var buttonId = $"remove-{productName.ToLower().Replace(" ", "-")}";
        await Page.Locator($"[data-test='{buttonId}']").ClickAsync();
    }

    public async Task<string> GetCartBadgeCount()
    {
        if (!await CartBadge.IsVisibleAsync()) return "0";
        return await CartBadge.TextContentAsync() ?? "0";
    }

    public async Task OpenCart()
    {
        await CartLink.ClickAsync();
        await Page.WaitForURLAsync("**/cart*");
    }

    public async Task SortBy(string value)
    {
        await SortDropdown.SelectOptionAsync(value);
    }

    public async Task<string> GetFirstProductName()
    {
        return await FirstItemName.TextContentAsync() ?? string.Empty;
    }

    public async Task<string> GetFirstProductPrice()
    {
        return await FirstItemPrice.TextContentAsync() ?? string.Empty;
    }
}
