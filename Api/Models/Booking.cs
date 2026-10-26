using System.Text.Json.Serialization;

namespace Quvantic.PlaywrightTests.Api.Models;

// Models double as the API contract: `required` members must be present in responses and
// ApiJson rejects unknown members, so any drift in the payload shape fails the test.

public sealed record Booking
{
    [JsonPropertyName("firstname")] public required string FirstName { get; init; }
    [JsonPropertyName("lastname")] public required string LastName { get; init; }
    [JsonPropertyName("totalprice")] public required int TotalPrice { get; init; }
    [JsonPropertyName("depositpaid")] public required bool DepositPaid { get; init; }
    [JsonPropertyName("bookingdates")] public required BookingDates BookingDates { get; init; }

    [JsonPropertyName("additionalneeds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AdditionalNeeds { get; init; }
}

public sealed record BookingDates
{
    [JsonPropertyName("checkin")] public required DateOnly CheckIn { get; init; }
    [JsonPropertyName("checkout")] public required DateOnly CheckOut { get; init; }
}

public sealed record CreatedBooking
{
    [JsonPropertyName("bookingid")] public required int BookingId { get; init; }
    [JsonPropertyName("booking")] public required Booking Booking { get; init; }
}

public sealed record BookingReference
{
    [JsonPropertyName("bookingid")] public required int BookingId { get; init; }
}
