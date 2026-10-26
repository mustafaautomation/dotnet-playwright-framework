using System.Net;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Api;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Utils;

namespace Quvantic.PlaywrightTests.Tests.Api.Booking;

[TestFixture]
[Category(TestCategories.Regression)]
public sealed class BookingNegativeTests : ApiTestBase
{
    [Test]
    public async Task GetBooking_UnknownId_Returns404()
    {
        var response = await Bookings.GetBookingAsync(int.MaxValue);

        await ShouldHaveStatusAsync(response, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateBooking_WithoutToken_Returns403()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());

        var response = await Bookings.UpdateBookingAsync(created.BookingId, BookingFactory.Create(), token: null);

        await ShouldHaveStatusAsync(response, HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DeleteBooking_WithInvalidToken_Returns403()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());

        var response = await Bookings.DeleteBookingAsync(created.BookingId, token: "invalid-token");

        await ShouldHaveStatusAsync(response, HttpStatusCode.Forbidden);
        await ShouldHaveStatusAsync(await Bookings.GetBookingAsync(created.BookingId), HttpStatusCode.OK);
    }

    [Test]
    public async Task CreateBooking_MissingRequiredFields_IsRejected()
    {
        var response = await Bookings.CreateBookingRawAsync(new { firstname = "OnlyAFirstName" });

        // The API has no input validation layer and surfaces this as a 500.
        await ShouldHaveStatusAsync(response, HttpStatusCode.InternalServerError);
    }
}
