using System.Net;

namespace Products.Api.Tests;

// PUT-5 · Optimistic concurrency with If-Match → 204 / 412 / 428 (extends PUT-1)
[Trait("Exercise", "PUT-5")]
public class Put5_OptimisticConcurrencyTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    private Task<HttpResponseMessage> PutLaptopAsync(int id, string? ifMatch, decimal price = 999m) =>
        SendAsync(HttpMethod.Put, $"/api/products/{id}", ifMatch is null ? null : "If-Match", ifMatch,
            Json(ProductBody(name: "Laptop", sku: "LAP-001", category: "Electronics", price: price, stock: 10)));

    [Fact]
    public async Task Matching_If_Match_returns_204_with_the_new_ETag()
    {
        var response = await PutLaptopAsync(1, "\"1\"");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("\"2\"", response.Headers.ETag?.Tag);
        var saved = Factory.FindProduct(1);
        Assert.Equal(999m, saved!.Price);
        Assert.Equal(2, saved.Version);
    }

    [Fact]
    public async Task Stale_If_Match_returns_412_and_does_not_overwrite()
    {
        await PutLaptopAsync(1, "\"1\"", price: 999m);

        var second = await PutLaptopAsync(1, "\"1\"", price: 555m);   // the version is now 2

        Assert.Equal(HttpStatusCode.PreconditionFailed, second.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal(999m, saved!.Price);
        Assert.Equal(2, saved.Version);
    }

    [Fact]
    public async Task Each_update_needs_the_ETag_from_the_previous_one()
    {
        var first = await PutLaptopAsync(1, "\"1\"");
        var second = await PutLaptopAsync(1, "\"2\"");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);
        Assert.Equal("\"3\"", second.Headers.ETag?.Tag);
    }

    [Fact]
    public async Task Star_If_Match_always_matches()
    {
        var response = await PutLaptopAsync(1, "*");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("\"2\"", response.Headers.ETag?.Tag);
    }

    [Fact]
    public async Task Missing_If_Match_returns_428_and_changes_nothing()
    {
        var response = await PutLaptopAsync(1, ifMatch: null);

        Assert.Equal((HttpStatusCode)428, response.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal(1200m, saved!.Price);
        Assert.Equal(1, saved.Version);
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await PutLaptopAsync(999, "\"1\"");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
