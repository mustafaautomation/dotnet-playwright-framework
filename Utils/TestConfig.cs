namespace Quvantic.PlaywrightTests.Utils;

public static class TestConfig
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "https://www.saucedemo.com";

    public static string StandardUser =>
        Environment.GetEnvironmentVariable("STANDARD_USER") ?? "standard_user";

    public static string Password =>
        Environment.GetEnvironmentVariable("PASSWORD") ?? "secret_sauce";

    public static int DefaultTimeout => 10_000;
}
