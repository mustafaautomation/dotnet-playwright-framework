using System.Net;
using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Api;
using Quvantic.PlaywrightTests.Api.Clients;
using Quvantic.PlaywrightTests.Api.Models;
using Quvantic.PlaywrightTests.Config;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Fixtures;

/// <summary>
/// Base fixture for API tests: one <see cref="IAPIRequestContext"/> per fixture, no browser.
/// Bookings created through <see cref="CreateBookingAsync"/> are deleted after the fixture.
/// </summary>
[Category(TestCategories.Api)]
public abstract class ApiTestBase
{
    private IPlaywright _playwright = null!;
    private IAPIRequestContext _request = null!;
    private readonly List<int> _createdBookingIds = [];

    protected static ApiSettings Settings => TestSettings.Current.Api;

    protected BookingClient Bookings { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task CreateRequestContextAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _request = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = Settings.BaseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string> { ["Accept"] = "application/json" }
        });
        Bookings = new BookingClient(_request);
    }

    [OneTimeTearDown]
    public async Task DisposeRequestContextAsync()
    {
        if (_createdBookingIds.Count > 0)
        {
            var token = await Bookings.GetTokenAsync(Settings.Username, Settings.Password);
            foreach (var id in _createdBookingIds)
            {
                await Bookings.DeleteBookingAsync(id, token); // best effort; may already be gone
            }
        }

        await _request.DisposeAsync();
        _playwright.Dispose();
    }

    protected Task<string> GetAdminTokenAsync() =>
        Bookings.GetTokenAsync(Settings.Username, Settings.Password);

    protected async Task<CreatedBooking> CreateBookingAsync(Booking booking)
    {
        var response = await Bookings.CreateBookingAsync(booking);
        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);

        var created = await response.ReadJsonAsync<CreatedBooking>();
        _createdBookingIds.Add(created.BookingId);
        return created;
    }

    /// <summary>Status assertion that includes the response body in the failure message.</summary>
    protected static async Task ShouldHaveStatusAsync(IAPIResponse response, HttpStatusCode expected)
    {
        if (response.Status != (int)expected)
        {
            var body = await response.TextAsync();
            response.Status.Should().Be((int)expected, "{0} returned body: {1}", response.Url, body);
        }
    }
}
