using Microsoft.Playwright;
using Quvantic.PlaywrightTests.Api.Models;

namespace Quvantic.PlaywrightTests.Api.Clients;

/// <summary>
/// Thin wrapper over the restful-booker endpoints. Methods return the raw
/// <see cref="IAPIResponse"/> so tests can assert on status codes, including negative cases;
/// callers deserialize with <see cref="ApiJson.ReadJsonAsync{T}"/>.
/// </summary>
public sealed class BookingClient(IAPIRequestContext request)
{
    public Task<IAPIResponse> CreateTokenAsync(string username, string password) =>
        request.PostAsync("/auth", Json(new AuthRequest(username, password)));

    public async Task<string> GetTokenAsync(string username, string password)
    {
        var response = await CreateTokenAsync(username, password);
        var token = await response.ReadJsonAsync<AuthToken>();
        return token.Token;
    }

    public Task<IAPIResponse> GetBookingIdsAsync(string? firstName = null, string? lastName = null)
    {
        var query = new Dictionary<string, object>();
        if (firstName is not null) query["firstname"] = firstName;
        if (lastName is not null) query["lastname"] = lastName;
        return request.GetAsync("/booking", new() { Params = query });
    }

    public Task<IAPIResponse> GetBookingAsync(int id) =>
        request.GetAsync($"/booking/{id}");

    public Task<IAPIResponse> CreateBookingAsync(Booking booking) =>
        request.PostAsync("/booking", Json(booking));

    public Task<IAPIResponse> CreateBookingRawAsync(object body) =>
        request.PostAsync("/booking", Json(body));

    public Task<IAPIResponse> UpdateBookingAsync(int id, Booking booking, string? token) =>
        request.PutAsync($"/booking/{id}", Json(booking, token));

    public Task<IAPIResponse> PartialUpdateBookingAsync(int id, object changes, string? token) =>
        request.PatchAsync($"/booking/{id}", Json(changes, token));

    public Task<IAPIResponse> DeleteBookingAsync(int id, string? token) =>
        request.DeleteAsync($"/booking/{id}", WithToken(new APIRequestContextOptions(), token));

    private static APIRequestContextOptions Json<T>(T body, string? token = null) =>
        WithToken(new APIRequestContextOptions
        {
            Data = ApiJson.Serialize(body),
            Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
        }, token);

    private static APIRequestContextOptions WithToken(APIRequestContextOptions options, string? token)
    {
        if (token is null) return options;

        var headers = new Dictionary<string, string>(options.Headers ?? []) { ["Cookie"] = $"token={token}" };
        options.Headers = headers;
        return options;
    }
}
