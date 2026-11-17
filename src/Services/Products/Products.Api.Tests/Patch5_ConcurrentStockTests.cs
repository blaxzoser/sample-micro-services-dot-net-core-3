using System.Net;

namespace Products.Api.Tests;

// PATCH-5 · Concurrent stock adjustment (thread safety) → 200 / 400 / 409
// Desk (id 3) starts with stock 5.
[Trait("Exercise", "PATCH-5")]
public class Patch5_ConcurrentStockTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    private Task<HttpResponseMessage> AdjustAsync(int id, int delta) =>
        Client.PatchAsync($"/api/products/{id}/stock", Json(new { delta }));

    [Fact]
    public async Task Negative_delta_within_stock_returns_200_with_productId_and_stock()
    {
        var response = await AdjustAsync(3, -2);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(3, body.GetProperty("productId").GetInt32());
        Assert.Equal(3, body.GetProperty("stock").GetInt32());
        Assert.Equal(3, Factory.FindProduct(3)!.Stock);
    }

    [Fact]
    public async Task Positive_delta_adds_stock()
    {
        var response = await AdjustAsync(3, 5);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(10, body.GetProperty("stock").GetInt32());
    }

    [Fact]
    public async Task Taking_all_the_stock_is_allowed()
    {
        var response = await AdjustAsync(3, -5);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, Factory.FindProduct(3)!.Stock);
    }

    [Fact]
    public async Task Going_below_zero_returns_409_and_leaves_the_stock_unchanged()
    {
        var response = await AdjustAsync(3, -10);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("Insufficient stock", body.GetProperty("title").GetString());
        Assert.Equal(5, Factory.FindProduct(3)!.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    [InlineData(-1001)]
    public async Task Zero_or_out_of_range_delta_returns_400(int delta)
    {
        var response = await AdjustAsync(3, delta);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(5, Factory.FindProduct(3)!.Stock);
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await AdjustAsync(999, -1);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Twenty_parallel_decrements_give_5_successes_15_conflicts_and_final_stock_0()
    {
        var responses = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => AdjustAsync(3, -1)));

        Assert.Equal(5, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Equal(15, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));
        Assert.Equal(0, Factory.FindProduct(3)!.Stock);
    }
}
