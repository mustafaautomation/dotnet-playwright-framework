using System.Text.Json.Serialization;

namespace Quvantic.PlaywrightTests.Api.Models;

public sealed record AuthRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

public sealed record AuthToken
{
    [JsonPropertyName("token")] public required string Token { get; init; }
}

/// <summary>restful-booker answers bad credentials with HTTP 200 and this body.</summary>
public sealed record AuthFailure
{
    [JsonPropertyName("reason")] public required string Reason { get; init; }
}
