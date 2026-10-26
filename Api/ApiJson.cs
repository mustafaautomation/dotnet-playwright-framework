using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Playwright;

namespace Quvantic.PlaywrightTests.Api;

public static class ApiJson
{
    /// <summary>
    /// Strict options: unknown members and missing <c>required</c> members both throw,
    /// which turns deserialization into a lightweight contract check.
    /// </summary>
    public static readonly JsonSerializerOptions StrictOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, StrictOptions);

    public static async Task<T> ReadJsonAsync<T>(this IAPIResponse response)
    {
        var body = await response.TextAsync();
        try
        {
            return JsonSerializer.Deserialize<T>(body, StrictOptions)
                ?? throw new JsonException("Response body was null.");
        }
        catch (JsonException ex)
        {
            throw new JsonException(
                $"{response.Status} {response.Url} does not match the {typeof(T).Name} contract: {ex.Message}\nBody: {body}", ex);
        }
    }
}
