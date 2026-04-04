using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Pages;

public class CheckoutPage : BasePage
{
    private ILocator FirstNameInput => Page.Locator("[data-test='firstName']");
    private ILocator LastNameInput => Page.Locator("[data-test='lastName']");
    private ILocator PostalCodeInput => Page.Locator("[data-test='postalCode']");
    private ILocator ContinueButton => Page.Locator("[data-test='continue']");
    private ILocator FinishButton => Page.Locator("[data-test='finish']");
    private ILocator CompleteHeader => Page.Locator(".complete-header");
    private ILocator TotalLabel => Page.Locator("[data-test='total-label']");
    private ILocator ErrorMessage => Page.Locator("[data-test='error']");

    public CheckoutPage(IPage page) : base(page) { }

    public async Task FillInfo(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task Continue()
    {
        await ContinueButton.ClickAsync();
    }

    public async Task Finish()
    {
        await FinishButton.ClickAsync();
    }

    public async Task<string> GetTotal()
    {
        return await TotalLabel.TextContentAsync() ?? string.Empty;
    }

    public async Task<bool> IsOrderComplete()
    {
        await CompleteHeader.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
        var text = await CompleteHeader.TextContentAsync() ?? string.Empty;
        return text.Contains("Thank you");
    }

    public async Task<string> GetErrorMessage()
    {
        await ErrorMessage.WaitForAsync();
        return await ErrorMessage.TextContentAsync() ?? string.Empty;
    }
}
