using System.Net;

namespace Products.Api.Tests;

// PUT-1 · Replace product → 204 / 400 / 404
// PUT-5 makes the If-Match header required (428). Once you finish PUT-5, the tests here
// that expect 204 or 400 without that header are expected to fail.
[Trait("Exercise", "PUT-1")]
public class Put1_ReplaceProductTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    private static object NewLaptop(string name = "Laptop Pro") =>
        ProductBody(name: name, sku: "LPR-002", category: "Computers", price: 999m, stock: 7);

    [Fact]
    public async Task Valid_body_returns_204_with_an_empty_body()
    {
        var response = await Client.PutAsync("/api/products/1", Json(NewLaptop()));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task Valid_body_replaces_all_fields_and_increments_the_version()
    {
        await Client.PutAsync("/api/products/1", Json(NewLaptop()));

        var saved = Factory.FindProduct(1);
        Assert.NotNull(saved);
        Assert.Equal("Laptop Pro", saved!.Name);
        Assert.Equal("LPR-002", saved.Sku);
        Assert.Equal("Computers", saved.Category);
        Assert.Equal(999m, saved.Price);
        Assert.Equal(7, saved.Stock);
        Assert.Equal(2, saved.Version);
    }

    [Fact]
    public async Task Get_after_the_put_shows_the_new_price_and_version_2()
    {
        await Client.PutAsync("/api/products/1", Json(NewLaptop()));

        var response = await Client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(999m, body.GetProperty("price").GetDecimal());
        Assert.Equal(2, body.GetProperty("version").GetInt32());
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await Client.PutAsync("/api/products/999", Json(NewLaptop()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Empty_name_returns_400_and_changes_nothing()
    {
        var response = await Client.PutAsync("/api/products/1", Json(NewLaptop(name: "")));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal("Laptop", saved!.Name);
        Assert.Equal(1200m, saved.Price);
        Assert.Equal(1, saved.Version);
    }
}
