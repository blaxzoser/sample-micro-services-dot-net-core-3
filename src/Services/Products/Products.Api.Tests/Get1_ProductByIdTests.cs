using System.Net;

namespace Products.Api.Tests;

// GET-1 · Product by id → 200 / 404
[Trait("Exercise", "GET-1")]
public class Get1_ProductByIdTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Existing_id_returns_200_with_the_product()
    {
        var response = await Client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(1, body.GetProperty("id").GetInt32());
        Assert.Equal("Laptop", body.GetProperty("name").GetString());
        Assert.Equal("LAP-001", body.GetProperty("sku").GetString());
        Assert.Equal(1200m, body.GetProperty("price").GetDecimal());
    }

    [Theory]
    [InlineData("999")]   // doesn't exist
    [InlineData("0")]     // doesn't exist
    [InlineData("abc")]   // not a number: needs the {id:int} route constraint, otherwise this is a 400
    public async Task Unknown_or_non_numeric_id_returns_404(string id)
    {
        var response = await Client.GetAsync($"/api/products/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // DELETE-4 changes this rule to 410 Gone. Once you finish DELETE-4, this test is expected to fail.
    [Fact]
    public async Task Soft_deleted_product_returns_404()
    {
        Factory.MarkDeleted(2);

        var response = await Client.GetAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
