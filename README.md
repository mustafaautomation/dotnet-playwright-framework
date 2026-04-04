# .NET Playwright Framework

[![E2E Tests](https://github.com/mustafaautomation/dotnet-playwright-framework/actions/workflows/e2e-tests.yml/badge.svg)](https://github.com/mustafaautomation/dotnet-playwright-framework/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4.svg?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![Playwright](https://img.shields.io/badge/Playwright-1.49-2EAD33.svg)](https://playwright.dev/dotnet)

Enterprise E2E testing framework built with Playwright for .NET and NUnit. Page Object Model architecture, parallel execution, FluentAssertions, and CI/CD integration.

---

## Test Coverage

| Suite | Tests | Category |
|-------|-------|----------|
| Login | 5 | smoke |
| Inventory | 5 | regression |
| Checkout | 3 | regression |
| **Total** | **13** | |

---

## Quick Start

```bash
git clone https://github.com/mustafaautomation/dotnet-playwright-framework.git
cd dotnet-playwright-framework

# Restore and build
dotnet restore
dotnet build

# Install browsers
pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps chromium

# Run all tests
dotnet test --settings PlaywrightTests.runsettings

# Run smoke tests only
dotnet test --filter "Category=smoke"

# Run regression tests
dotnet test --filter "Category=regression"
```

---

## Architecture

```
┌─────────────────────────────────────────┐
│           Test Classes                   │
│  LoginTests │ InventoryTests │ ...       │
├─────────────────────────────────────────┤
│           BaseTest                       │
│  Page init │ LoginAsStandardUser()       │
├─────────────────────────────────────────┤
│           Page Objects                   │
│  LoginPage │ InventoryPage │ CartPage    │
│  CheckoutPage │ BasePage                 │
├─────────────────────────────────────────┤
│           Utils                          │
│  TestConfig (env-based)                  │
├─────────────────────────────────────────┤
│  Playwright .NET │ NUnit │ FluentAssert  │
└─────────────────────────────────────────┘
```

---

## Page Object Model

Each page encapsulates selectors and actions:

```csharp
public class LoginPage : BasePage
{
    private ILocator UsernameInput => Page.Locator("[data-test='username']");

    public async Task Login(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
```

---

## Parallel Execution

Tests run in parallel by default (4 workers). Configure in `PlaywrightTests.runsettings`:

```xml
<NUnit>
  <NumberOfTestWorkers>4</NumberOfTestWorkers>
</NUnit>
```

Each test class gets `[Parallelizable(ParallelScope.Self)]` for thread safety.

---

## Docker

```bash
docker build -t pw-dotnet .
docker run --rm pw-dotnet
docker run --rm pw-dotnet --filter "Category=smoke"
```

---

## Project Structure

```
dotnet-playwright-framework/
├── Pages/
│   ├── BasePage.cs              # Base with screenshot, URL helpers
│   ├── LoginPage.cs             # Login form interactions
│   ├── InventoryPage.cs         # Product list, cart, sorting
│   ├── CartPage.cs              # Cart operations
│   └── CheckoutPage.cs          # Checkout form and validation
├── Tests/
│   ├── BaseTest.cs              # NUnit setup, page initialization
│   ├── Auth/LoginTests.cs       # 5 login scenarios
│   ├── Inventory/InventoryTests.cs  # 5 inventory scenarios
│   └── Checkout/CheckoutTests.cs    # 3 checkout scenarios
├── Utils/
│   └── TestConfig.cs            # Environment-based configuration
├── PlaywrightTests.csproj       # .NET 8 project file
├── PlaywrightTests.runsettings  # Playwright + NUnit config
├── Dockerfile
└── .github/workflows/e2e-tests.yml
```

---

## Stack

| Tool | Purpose |
|------|---------|
| Playwright .NET 1.49 | Browser automation |
| NUnit 4.3 | Test runner with parallel support |
| FluentAssertions 7.0 | Readable C# assertions |
| .NET 8 | Runtime |
| Docker | Containerized execution |

---

## License

MIT

---

Built by [Quvantic](https://quvantic.com)
