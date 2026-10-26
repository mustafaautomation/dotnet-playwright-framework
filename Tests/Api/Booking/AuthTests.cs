using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Api;
using Quvantic.PlaywrightTests.Api.Models;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Tests.Api.Booking;

[TestFixture]
public sealed class AuthTests : ApiTestBase
{
    [Test, Category(TestCategories.Smoke)]
    public async Task ValidCredentials_ReturnToken()
    {
        var response = await Bookings.CreateTokenAsync(Settings.Username, Settings.Password);

        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);
        var auth = await response.ReadJsonAsync<AuthToken>();
        auth.Token.Should().MatchRegex("^[a-f0-9]{15}$");
    }

    [Test, Category(TestCategories.Regression)]
    public async Task InvalidCredentials_ReturnBadCredentialsReason()
    {
        var response = await Bookings.CreateTokenAsync(Settings.Username, "not-the-password");

        // restful-booker reports auth failure in the body with HTTP 200 (documented behaviour).
        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);
        var failure = await response.ReadJsonAsync<AuthFailure>();
        failure.Reason.Should().Be("Bad credentials");
    }
}
