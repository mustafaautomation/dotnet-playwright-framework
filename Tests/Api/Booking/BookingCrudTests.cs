using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Quvantic.PlaywrightTests.Api;
using Quvantic.PlaywrightTests.Api.Models;
using Quvantic.PlaywrightTests.Fixtures;
using Quvantic.PlaywrightTests.Utils;
using BookingModel = Quvantic.PlaywrightTests.Api.Models.Booking;

namespace Quvantic.PlaywrightTests.Tests.Api.Booking;

[TestFixture]
public sealed class BookingCrudTests : ApiTestBase
{
    [Test, Category(TestCategories.Smoke)]
    public async Task CreateBooking_ReturnsIdAndEchoesBooking()
    {
        var booking = BookingFactory.Create();

        var created = await CreateBookingAsync(booking);

        created.BookingId.Should().BePositive();
        created.Booking.Should().Be(booking);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task GetBooking_ReturnsStoredBookingMatchingContract()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());

        var response = await Bookings.GetBookingAsync(created.BookingId);

        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);
        var fetched = await response.ReadJsonAsync<BookingModel>(); // strict: fails on schema drift
        fetched.Should().Be(created.Booking);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task GetBookingIds_FilteredByName_ContainsCreatedBooking()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());

        var response = await Bookings.GetBookingIdsAsync(created.Booking.FirstName, created.Booking.LastName);

        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);
        var ids = await response.ReadJsonAsync<List<BookingReference>>();
        ids.Select(b => b.BookingId).Should().Contain(created.BookingId);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task UpdateBooking_WithToken_ReplacesBooking()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());
        var token = await GetAdminTokenAsync();
        var replacement = BookingFactory.Create(additionalNeeds: "Late checkout") with { DepositPaid = false };

        var response = await Bookings.UpdateBookingAsync(created.BookingId, replacement, token);

        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);
        (await response.ReadJsonAsync<BookingModel>()).Should().Be(replacement);

        var stored = await (await Bookings.GetBookingAsync(created.BookingId)).ReadJsonAsync<BookingModel>();
        stored.Should().Be(replacement);
    }

    [Test, Category(TestCategories.Regression)]
    public async Task PartialUpdateBooking_ChangesOnlyGivenFields()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());
        var token = await GetAdminTokenAsync();

        var response = await Bookings.PartialUpdateBookingAsync(
            created.BookingId, new { firstname = "Patched", totalprice = 42 }, token);

        await ShouldHaveStatusAsync(response, HttpStatusCode.OK);
        var updated = await response.ReadJsonAsync<BookingModel>();
        updated.Should().Be(created.Booking with { FirstName = "Patched", TotalPrice = 42 });
    }

    [Test, Category(TestCategories.Regression)]
    public async Task DeleteBooking_WithToken_RemovesBooking()
    {
        var created = await CreateBookingAsync(BookingFactory.Create());
        var token = await GetAdminTokenAsync();

        var response = await Bookings.DeleteBookingAsync(created.BookingId, token);

        // restful-booker returns 201 Created for a successful delete (documented behaviour).
        await ShouldHaveStatusAsync(response, HttpStatusCode.Created);
        await ShouldHaveStatusAsync(await Bookings.GetBookingAsync(created.BookingId), HttpStatusCode.NotFound);
    }
}
