using System.Net;

namespace Products.Api.Tests;

// DELETE-5 · API-key protected delete → 401 / 403 / 204
// Keys: "reader-key" → role reader, "admin-key" → role admin.
[Trait("Exercise", "DELETE-5")]
public class Delete5_ApiKeyProtectedDeleteTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    private Task<HttpResponseMessage> DeleteAsync(int id, string? apiKey) =>
        SendAsync(HttpMethod.Delete, $"/api/products/{id}", apiKey is null ? null : "X-Api-Key", apiKey);

    private void AssertStillThere(int id)
    {
        var row = Factory.FindProduct(id);
        Assert.NotNull(row);
        Assert.False(row!.IsDeleted);
    }

    [Fact]
    public async Task No_key_returns_401_with_WWW_Authenticate_ApiKey_and_deletes_nothing()
    {
        var response = await DeleteAsync(1, apiKey: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains(response.Headers.WwwAuthenticate, h => h.Scheme == "ApiKey");
        AssertStillThere(1);
    }

    [Fact]
    public async Task Unknown_key_returns_401_with_WWW_Authenticate_ApiKey()
    {
        var response = await DeleteAsync(1, "wrong");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains(response.Headers.WwwAuthenticate, h => h.Scheme == "ApiKey");
        AssertStillThere(1);
    }

    [Fact]
    public async Task Reader_key_returns_403_ProblemDetails_and_deletes_nothing()
    {
        var response = await DeleteAsync(1, "reader-key");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(403, body.GetProperty("status").GetInt32());
        AssertStillThere(1);
    }

    [Fact]
    public async Task Admin_key_deletes_the_product_and_returns_204()
    {
        var response = await DeleteAsync(1, "admin-key");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var row = Factory.FindProduct(1);
        Assert.True(row is null || row.IsDeleted, "Expected the product to be removed or soft-deleted.");
    }

    // Auth runs before the action, so the response must not reveal whether the product exists.
    [Fact]
    public async Task Reader_key_on_an_unknown_id_returns_403_not_404()
    {
        var response = await DeleteAsync(999, "reader-key");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task No_key_on_an_unknown_id_returns_401_not_404()
    {
        var response = await DeleteAsync(999, apiKey: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Admin_key_on_an_unknown_id_returns_404()
    {
        var response = await DeleteAsync(999, "admin-key");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
