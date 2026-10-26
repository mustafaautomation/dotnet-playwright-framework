using System.Globalization;
using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public sealed record OrderSummary(decimal ItemTotal, decimal Tax, decimal Total);

public sealed class CheckoutPage(IPage page) : BasePage(page)
{
    private ILocator FirstNameInput => ByTestId("firstName");
    private ILocator LastNameInput => ByTestId("lastName");
    private ILocator PostalCodeInput => ByTestId("postalCode");
    private ILocator ContinueButton => ByTestId("continue");
    private ILocator FinishButton => ByTestId("finish");
    private ILocator ItemTotalLabel => ByTestId("subtotal-label");
    private ILocator TaxLabel => ByTestId("tax-label");
    private ILocator TotalLabel => ByTestId("total-label");

    public ILocator ErrorMessage => ByTestId("error");
    public ILocator CompleteHeader => ByTestId("complete-header");

    public async Task FillCustomerInfoAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueAsync() => await ContinueButton.ClickAsync();

    public async Task FinishAsync() => await FinishButton.ClickAsync();

    public async Task<OrderSummary> GetOrderSummaryAsync() => new(
        await ReadAmountAsync(ItemTotalLabel),
        await ReadAmountAsync(TaxLabel),
        await ReadAmountAsync(TotalLabel));

    // Labels look like "Item total: $29.99".
    private static async Task<decimal> ReadAmountAsync(ILocator label)
    {
        var text = await label.InnerTextAsync();
        return decimal.Parse(text[(text.IndexOf('$') + 1)..], CultureInfo.InvariantCulture);
    }
}
