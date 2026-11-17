using System.Net;

namespace Products.Api.Tests;

// GET-5 · Conditional GET with ETag → 200 / 304 (extends GET-1)
[Trait("Exercise", "GET-5")]
public class Get5_ConditionalGetETagTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Plain_get_returns_200_with_ETag_and_Cache_Control()
    {
        var response = await Client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("\"1\"", response.Headers.ETag?.Tag);
        Assert.NotNull(response.Headers.CacheControl);
        Assert.True(response.Headers.CacheControl!.Private, "Expected Cache-Control: private.");
        Assert.Equal(TimeSpan.FromSeconds(60), response.Headers.CacheControl.MaxAge);
    }

    [Theory]
    [InlineData("\"1\"")]   // matches the current ETag
    [InlineData("*")]       // matches anything
    public async Task Matching_If_None_Match_returns_304_with_no_body_and_the_ETag(string ifNoneMatch)
    {
        var response = await SendAsync(HttpMethod.Get, "/api/products/1", "If-None-Match", ifNoneMatch);

        Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
        Assert.Equal("\"1\"", response.Headers.ETag?.Tag);
    }

    [Fact]
    public async Task Different_If_None_Match_returns_200_with_the_body_and_the_current_ETag()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/products/1", "If-None-Match", "\"0\"");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("\"1\"", response.Headers.ETag?.Tag);
        var body = await ReadJsonAsync(response);
        Assert.Equal("Laptop", body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Unknown_id_returns_404_even_with_If_None_Match()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/products/999", "If-None-Match", "\"1\"");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
