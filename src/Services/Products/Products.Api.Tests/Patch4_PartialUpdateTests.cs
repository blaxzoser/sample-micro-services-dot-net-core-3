using System.Net;
using System.Text;

namespace Products.Api.Tests;

// PATCH-4 · Partial product update (only the fields sent) → 200 / 400 / 404
[Trait("Exercise", "PATCH-4")]
public class Patch4_PartialUpdateTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Only_price_sent_returns_200_and_changes_only_the_price()
    {
        var response = await Client.PatchAsync("/api/products/1", Json(new { price = 999 }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(999m, body.GetProperty("price").GetDecimal());
        Assert.Equal("Laptop", body.GetProperty("name").GetString());

        var saved = Factory.FindProduct(1);
        Assert.Equal(999m, saved!.Price);
        Assert.Equal("Laptop", saved.Name);
        Assert.Equal("Electronics", saved.Category);
        Assert.Equal(10, saved.Stock);
        Assert.Equal(2, saved.Version);
    }

    [Fact]
    public async Task Several_fields_sent_change_only_those_fields()
    {
        var response = await Client.PatchAsync("/api/products/2", Json(new { name = "Pro", stock = 0 }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = Factory.FindProduct(2);
        Assert.Equal("Pro", saved!.Name);
        Assert.Equal(0, saved.Stock);
        Assert.Equal(25m, saved.Price);   // not sent, unchanged
    }

    [Fact]
    public async Task Stock_zero_is_a_real_value_and_not_treated_as_not_sent()
    {
        var response = await Client.PatchAsync("/api/products/1", Json(new { stock = 0 }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, Factory.FindProduct(1)!.Stock);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"sku":"XXX-999"}""")]   // sku is not patchable, so nothing to update
    [InlineData("""{"id":50}""")]           // id is not patchable either
    public async Task Body_without_patchable_fields_returns_400_and_changes_nothing(string json)
    {
        var response = await Client.PatchAsync("/api/products/1", Raw(json));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal(1, saved!.Id);
        Assert.Equal("LAP-001", saved.Sku);
        Assert.Equal(1, saved.Version);
    }

    [Theory]
    [InlineData("""{"price":-1}""")]
    [InlineData("""{"price":0}""")]
    [InlineData("""{"stock":-5}""")]
    [InlineData("""{"name":""}""")]
    public async Task Invalid_value_returns_400_and_changes_nothing(string json)
    {
        var response = await Client.PatchAsync("/api/products/1", Raw(json));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal("Laptop", saved!.Name);
        Assert.Equal(1200m, saved.Price);
        Assert.Equal(10, saved.Stock);
        Assert.Equal(1, saved.Version);
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await Client.PatchAsync("/api/products/999", Json(new { price = 10 }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Soft_deleted_product_returns_404()
    {
        Factory.MarkDeleted(1);

        var response = await Client.PatchAsync("/api/products/1", Json(new { price = 10 }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(1200m, Factory.FindProduct(1)!.Price);
    }

    private static StringContent Raw(string json) => new(json, Encoding.UTF8, "application/json");
}
