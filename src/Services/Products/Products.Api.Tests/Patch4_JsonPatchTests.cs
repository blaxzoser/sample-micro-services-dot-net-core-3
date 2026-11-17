using System.Net;

namespace Products.Api.Tests;

// PATCH-4 · JSON Patch document (RFC 6902) → 200 / 400 / 404 / 415
// Needs the Microsoft.AspNetCore.Mvc.NewtonsoftJson package and AddNewtonsoftJson() in Program.cs.
[Trait("Exercise", "PATCH-4")]
public class Patch4_JsonPatchTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    private const string ReplacePrice = """[{"op":"replace","path":"/price","value":999}]""";

    [Fact]
    public async Task Replace_price_returns_200_with_the_product_and_increments_the_version()
    {
        var response = await Client.PatchAsync("/api/products/1", JsonPatch(ReplacePrice));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(999m, body.GetProperty("price").GetDecimal());
        Assert.Equal("Laptop", body.GetProperty("name").GetString());

        var saved = Factory.FindProduct(1);
        Assert.Equal(999m, saved!.Price);
        Assert.Equal(2, saved.Version);
    }

    [Fact]
    public async Task Several_operations_are_applied_together()
    {
        var patch = """[{"op":"replace","path":"/name","value":"Pro"},{"op":"replace","path":"/stock","value":0}]""";

        var response = await Client.PatchAsync("/api/products/1", JsonPatch(patch));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal("Pro", saved!.Name);
        Assert.Equal(0, saved.Stock);
        Assert.Equal(1200m, saved.Price);   // untouched fields keep their values
    }

    [Theory]
    [InlineData("""[{"op":"replace","path":"/sku","value":"XXX-999"}]""")]   // not patchable
    [InlineData("""[{"op":"replace","path":"/id","value":50}]""")]           // not patchable
    [InlineData("""[{"op":"replace","path":"/price","value":-1}]""")]        // invalid after the patch
    [InlineData("""[{"op":"replace","path":"/name","value":""}]""")]         // invalid after the patch
    public async Task Unknown_path_or_invalid_result_returns_400_and_changes_nothing(string patch)
    {
        var response = await Client.PatchAsync("/api/products/1", JsonPatch(patch));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var saved = Factory.FindProduct(1);
        Assert.Equal(1, saved!.Id);
        Assert.Equal("Laptop", saved.Name);
        Assert.Equal("LAP-001", saved.Sku);
        Assert.Equal(1200m, saved.Price);
        Assert.Equal(1, saved.Version);
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await Client.PatchAsync("/api/products/999", JsonPatch(ReplacePrice));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Wrong_content_type_returns_415()
    {
        var response = await Client.PatchAsync("/api/products/1", JsonPatch(ReplacePrice, contentType: "application/json"));

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
}
