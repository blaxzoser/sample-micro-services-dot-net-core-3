using System.Net;

namespace Products.Api.Tests;

// POST-3 · Reject duplicate SKU → 409 Conflict (extends POST-2)
[Trait("Exercise", "POST-3")]
public class Post3_DuplicateSkuTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Existing_sku_returns_409_ProblemDetails_naming_the_sku()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(sku: "LAP-001")));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("Duplicate SKU", body.GetProperty("title").GetString());
        Assert.Contains("LAP-001", body.GetProperty("detail").GetString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Duplicate_check_is_case_insensitive()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(sku: "lap-001")));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Duplicate_does_not_add_a_row()
    {
        await Client.PostAsync("/api/products", Json(ProductBody(sku: "LAP-001")));

        Assert.Null(Factory.FindProduct(4));
    }

    [Fact]
    public async Task New_sku_returns_201_and_the_same_sku_again_returns_409()
    {
        var first = await Client.PostAsync("/api/products", Json(ProductBody(sku: "NEW-001")));
        var second = await Client.PostAsync("/api/products", Json(ProductBody(sku: "NEW-001")));

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Two_parallel_requests_with_the_same_new_sku_give_one_201_and_one_409()
    {
        var responses = await Task.WhenAll(
            Client.PostAsync("/api/products", Json(ProductBody(sku: "NEW-001"))),
            Client.PostAsync("/api/products", Json(ProductBody(sku: "NEW-001"))));

        var codes = responses.Select(r => r.StatusCode).OrderBy(c => c).ToArray();
        Assert.Equal(new[] { HttpStatusCode.Created, HttpStatusCode.Conflict }, codes);
    }
}
