using Quvantic.PlaywrightTests.Api.Models;

namespace Quvantic.PlaywrightTests.Api;

/// <summary>Builds unique, valid bookings so parallel runs against a shared API don't collide.</summary>
public static class BookingFactory
{
    public static Booking Create(string? additionalNeeds = "Breakfast")
    {
        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(Random.Shared.Next(30, 300));

        return new Booking
        {
            FirstName = "Test",
            LastName = $"User-{Guid.NewGuid():N}"[..17],
            TotalPrice = Random.Shared.Next(50, 1000),
            DepositPaid = true,
            BookingDates = new BookingDates { CheckIn = checkIn, CheckOut = checkIn.AddDays(3) },
            AdditionalNeeds = additionalNeeds
        };
    }
}
