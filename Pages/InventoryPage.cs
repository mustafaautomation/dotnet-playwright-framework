using System.Globalization;
using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public enum SortOption
{
    NameAscending,
    NameDescending,
    PriceAscending,
    PriceDescending
}

public sealed class InventoryPage(IPage page) : BasePage(page)
{
    private ILocator ItemNames => Page.Locator(".inventory_item_name");
    private ILocator ItemPrices => Page.Locator(".inventory_item_price");
    private ILocator CartLink => Page.Locator(".shopping_cart_link");
    private ILocator SortDropdown => ByTestId("product-sort-container");

    public ILocator Items => Page.Locator(".inventory_item");
    public ILocator CartBadge => Page.Locator(".shopping_cart_badge");

    public async Task AddToCartAsync(string productName) =>
        await ByTestId($"add-to-cart-{ToSlug(productName)}").ClickAsync();

    public async Task RemoveFromCartAsync(string productName) =>
        await ByTestId($"remove-{ToSlug(productName)}").ClickAsync();

    public async Task OpenCartAsync() => await CartLink.ClickAsync();

    public async Task SortByAsync(SortOption option)
    {
        var value = option switch
        {
            SortOption.NameAscending => "az",
            SortOption.NameDescending => "za",
            SortOption.PriceAscending => "lohi",
            SortOption.PriceDescending => "hilo",
            _ => throw new ArgumentOutOfRangeException(nameof(option))
        };
        await SortDropdown.SelectOptionAsync(value);
    }

    public async Task<IReadOnlyList<string>> GetProductNamesAsync() =>
        await ItemNames.AllTextContentsAsync();

    public async Task<IReadOnlyList<decimal>> GetProductPricesAsync()
    {
        var prices = await ItemPrices.AllTextContentsAsync();
        return prices.Select(p => decimal.Parse(p.TrimStart('$'), CultureInfo.InvariantCulture)).ToList();
    }

    private static string ToSlug(string productName) =>
        productName.ToLowerInvariant().Replace(' ', '-');
}
