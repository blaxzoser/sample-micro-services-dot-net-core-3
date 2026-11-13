using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Feedback.Api.Tests;

/// <summary>
/// POST-1 · Submit feedback -> 200 / 400 (doc/aspnet-mock-interview-exercises.md).
/// xUnit creates a new instance of this class per test, so every test gets a fresh
/// API host and an empty database.
/// </summary>
public class SubmitFeedbackTests : IDisposable
{
    private const string Url = "/api/feedback";

    private readonly FeedbackApiFactory _factory = new();
    private readonly HttpClient _client;

    public SubmitFeedbackTests() => _client = _factory.CreateClient();

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // ---------- 200 OK ----------

    [Fact]
    public async Task Valid_feedback_returns_200_with_id_1_and_the_stored_values()
    {
        var before = DateTime.UtcNow.AddSeconds(-5);

        var response = await _client.PostAsJsonAsync(Url, new { message = "Great", rating = 5 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJson(response);
        Assert.Equal(1, body.GetProperty("id").GetInt32());
        Assert.Equal("Great", body.GetProperty("message").GetString());
        Assert.Equal(5, body.GetProperty("rating").GetInt32());
        var receivedAt = body.GetProperty("receivedAt").GetDateTime().ToUniversalTime();
        Assert.InRange(receivedAt, before, DateTime.UtcNow.AddSeconds(5));
    }

    [Fact]
    public async Task Second_feedback_returns_200_with_id_2()
    {
        await _client.PostAsJsonAsync(Url, new { message = "Great", rating = 5 });

        var response = await _client.PostAsJsonAsync(Url, new { message = "Good", rating = 4 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJson(response);
        Assert.Equal(2, body.GetProperty("id").GetInt32());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public async Task Rating_on_the_boundary_is_accepted(int rating)
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = "Ok", rating });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Message_with_exactly_500_characters_is_accepted()
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = new string('a', 500), rating = 3 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Valid_feedback_is_saved_to_the_database()
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = "Great", rating = 5 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = _factory.WithDb(db => db.Feedbacks.ToList());
        var entry = Assert.Single(saved);
        Assert.Equal(1, entry.Id);
        Assert.Equal("Great", entry.Message);
        Assert.Equal(5, entry.Rating);
    }

    // ---------- 400 Bad Request ----------

    [Fact]
    public async Task Empty_message_returns_400_with_a_Message_error()
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = "", rating = 3 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await ReadJson(response);
        Assert.True(body.GetProperty("errors").TryGetProperty("Message", out _), "errors.Message is missing");
    }

    [Fact]
    public async Task Whitespace_only_message_returns_400()
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = "   ", rating = 3 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Message_longer_than_500_characters_returns_400()
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = new string('a', 501), rating = 3 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(9)]
    [InlineData(-1)]
    public async Task Rating_out_of_range_returns_400_with_a_Rating_error(int rating)
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = "Ok", rating });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await ReadJson(response);
        Assert.True(body.GetProperty("errors").TryGetProperty("Rating", out _), "errors.Rating is missing");
    }

    [Fact]
    public async Task Missing_rating_returns_400()
    {
        // rating defaults to 0, which is outside 1..5
        var response = await _client.PostAsJsonAsync(Url, new { message = "Ok" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Body_that_is_not_json_returns_400()
    {
        var content = new StringContent("not json", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_feedback_is_not_saved_to_the_database()
    {
        var response = await _client.PostAsJsonAsync(Url, new { message = "", rating = 9 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.WithDb(db => db.Feedbacks.Count()));
    }

    // ---------- 415 Unsupported Media Type ----------

    [Fact]
    public async Task Text_plain_content_type_returns_415()
    {
        var content = new StringContent("Great", Encoding.UTF8, "text/plain");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    private static async Task<JsonElement> ReadJson(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.Clone();
    }
}
