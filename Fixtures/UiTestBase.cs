using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Quvantic.PlaywrightTests.Config;
using Quvantic.PlaywrightTests.Pages;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Fixtures;

/// <summary>
/// Base fixture for UI tests. One browser per fixture, one fresh <see cref="IBrowserContext"/>
/// per test (isolated cookies/storage). Captures a trace and screenshot when a test fails
/// and attaches them to the NUnit result.
/// </summary>
[Category(TestCategories.Ui)]
public abstract class UiTestBase
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    protected static UiSettings Settings => TestSettings.Current.Ui;
    private static DiagnosticsSettings Diagnostics => TestSettings.Current.Diagnostics;

    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    protected LoginPage LoginPage { get; private set; } = null!;
    protected InventoryPage InventoryPage { get; private set; } = null!;
    protected CartPage CartPage { get; private set; } = null!;
    protected CheckoutPage CheckoutPage { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task LaunchBrowserAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Assertions.SetDefaultExpectTimeout(Settings.TimeoutMs);

        var browserType = Settings.Browser.ToLowerInvariant() switch
        {
            "chromium" => _playwright.Chromium,
            "firefox" => _playwright.Firefox,
            "webkit" => _playwright.Webkit,
            var other => throw new ArgumentException($"Unsupported browser '{other}'.")
        };

        _browser = await browserType.LaunchAsync(new() { Headless = Settings.Headless });
    }

    [SetUp]
    public async Task CreateContextAsync()
    {
        Context = await _browser.NewContextAsync(new()
        {
            BaseURL = Settings.BaseUrl,
            ViewportSize = new() { Width = 1280, Height = 720 }
        });
        Context.SetDefaultTimeout(Settings.TimeoutMs);

        if (Diagnostics.Tracing != TraceMode.Off)
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = TestContext.CurrentContext.Test.FullName,
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        Page = await Context.NewPageAsync();
        LoginPage = new LoginPage(Page);
        InventoryPage = new InventoryPage(Page);
        CartPage = new CartPage(Page);
        CheckoutPage = new CheckoutPage(Page);
    }

    [TearDown]
    public async Task CloseContextAsync()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var artifactPath = ArtifactPathFor(TestContext.CurrentContext.Test);

        try
        {
            if (failed && Diagnostics.ScreenshotOnFailure)
            {
                var screenshot = $"{artifactPath}.png";
                await Page.ScreenshotAsync(new() { Path = screenshot, FullPage = true });
                TestContext.AddTestAttachment(screenshot, "Screenshot at failure");
            }

            if (Diagnostics.Tracing != TraceMode.Off)
            {
                var keepTrace = failed || Diagnostics.Tracing == TraceMode.On;
                var trace = $"{artifactPath}.trace.zip";
                await Context.Tracing.StopAsync(new() { Path = keepTrace ? trace : null });
                if (keepTrace)
                {
                    TestContext.AddTestAttachment(trace, "Playwright trace (npx playwright show-trace <file>)");
                }
            }
        }
        catch (PlaywrightException ex)
        {
            // Never let diagnostics collection hide the real test outcome.
            TestContext.Progress.WriteLine($"Could not collect diagnostics: {ex.Message}");
        }
        finally
        {
            await Context.CloseAsync();
        }
    }

    [OneTimeTearDown]
    public async Task CloseBrowserAsync()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    protected async Task LoginAsStandardUserAsync()
    {
        await LoginPage.GotoAsync();
        await LoginPage.LoginAsync(Settings.Username, Settings.Password);
        await Assertions.Expect(InventoryPage.Items.First).ToBeVisibleAsync();
    }

    private static string ArtifactPathFor(TestContext.TestAdapter test)
    {
        var directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-artifacts");
        Directory.CreateDirectory(directory);

        var safeName = string.Concat(test.Name.Select(c => Path.GetInvalidFileNameChars().Contains(c) || c == '"' ? '_' : c));
        return Path.Combine(directory, $"{test.ClassName?.Split('.').Last()}.{safeName}");
    }
}
