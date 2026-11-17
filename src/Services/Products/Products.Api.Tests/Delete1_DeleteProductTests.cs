using System.Net;

namespace Products.Api.Tests;

// DELETE-1 · Delete product (hard delete) → 204 / 404
// DELETE-4 (soft delete, 410) and DELETE-5 (API key, 401) change these rules. Once you
// finish either of them, most tests here are expected to fail.
[Trait("Exercise", "DELETE-1")]
public class Delete1_DeleteProductTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Existing_product_returns_204_with_an_empty_body_and_removes_the_row()
    {
        var response = await Client.DeleteAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
        Assert.Null(Factory.FindProduct(2));
    }

    [Fact]
    public async Task Get_after_the_delete_returns_404()
    {
        var deleted = await Client.DeleteAsync("/api/products/2");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var response = await Client.GetAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Deleting_the_same_product_again_returns_404()
    {
        var first = await Client.DeleteAsync("/api/products/2");
        var second = await Client.DeleteAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
    }

    [Theory]
    [InlineData("999")]
    [InlineData("abc")]   // needs the {id:int} route constraint
    public async Task Unknown_or_non_numeric_id_returns_404(string id)
    {
        var response = await Client.DeleteAsync($"/api/products/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Other_products_are_not_touched()
    {
        await Client.DeleteAsync("/api/products/2");

        Assert.NotNull(Factory.FindProduct(1));
        Assert.NotNull(Factory.FindProduct(3));
    }
}
