using NUnit.Framework;

// Fixtures run in parallel; tests inside a fixture run sequentially and share the
// fixture's browser (UI) or request context (API). Each UI test still gets its own
// isolated BrowserContext. Worker count is set in PlaywrightTests.runsettings.
[assembly: Parallelizable(ParallelScope.Fixtures)]
