using Microsoft.Extensions.Configuration;

namespace Quvantic.PlaywrightTests.Config;

/// <summary>
/// Strongly typed test settings. Values come from appsettings.json, an optional
/// appsettings.{TEST_ENVIRONMENT}.json, and finally environment variables prefixed
/// with <c>TEST_</c> (e.g. <c>TEST_Ui__Browser=firefox</c>), in that order of precedence.
/// </summary>
public sealed class TestSettings
{
    private const string EnvPrefix = "TEST_";

    private static readonly Lazy<TestSettings> Instance = new(Load);

    public static TestSettings Current => Instance.Value;

    public UiSettings Ui { get; init; } = new();
    public ApiSettings Api { get; init; } = new();
    public DiagnosticsSettings Diagnostics { get; init; } = new();

    private static TestSettings Load()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false);

        var environment = Environment.GetEnvironmentVariable($"{EnvPrefix}ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(environment))
        {
            builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
        }

        var configuration = builder.AddEnvironmentVariables(EnvPrefix).Build();

        return configuration.Get<TestSettings>()
            ?? throw new InvalidOperationException("Test settings could not be loaded.");
    }
}

public sealed class UiSettings
{
    public string BaseUrl { get; init; } = "https://www.saucedemo.com";
    public string Browser { get; init; } = "chromium";
    public bool Headless { get; init; } = true;
    public int TimeoutMs { get; init; } = 10_000;
    public string Username { get; init; } = "standard_user";
    public string Password { get; init; } = "secret_sauce";
}

public sealed class ApiSettings
{
    public string BaseUrl { get; init; } = "https://restful-booker.herokuapp.com";
    public string Username { get; init; } = "admin";
    public string Password { get; init; } = "password123";
}

public sealed class DiagnosticsSettings
{
    public TraceMode Tracing { get; init; } = TraceMode.RetainOnFailure;
    public bool ScreenshotOnFailure { get; init; } = true;
}

public enum TraceMode
{
    Off,
    On,
    RetainOnFailure
}
