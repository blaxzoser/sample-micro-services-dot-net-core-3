using System.Net;

namespace Products.Api.Tests;

// DELETE-4 · Soft delete + 410 Gone
// DELETE-5 puts an API key in front of DELETE (401). Once you finish DELETE-5, the tests
// here that call DELETE are expected to fail.
[Trait("Exercise", "DELETE-4")]
public class Delete4_SoftDeleteTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Delete_returns_204_and_keeps_the_row_marked_as_deleted()
    {
        var response = await Client.DeleteAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var row = Factory.FindProduct(2);
        Assert.NotNull(row);              // the row is still in the table
        Assert.True(row!.IsDeleted);
    }

    [Fact]
    public async Task Get_on_a_deleted_product_returns_410()
    {
        await Client.DeleteAsync("/api/products/2");

        var response = await Client.GetAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
    }

    [Fact]
    public async Task Delete_on_an_already_deleted_product_returns_410()
    {
        var first = await Client.DeleteAsync("/api/products/2");
        var second = await Client.DeleteAsync("/api/products/2");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.Gone, second.StatusCode);
    }

    [Fact]
    public async Task List_leaves_deleted_products_out()
    {
        await Client.DeleteAsync("/api/products/2");

        var response = await Client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(2, body.GetArrayLength());
        Assert.DoesNotContain(body.EnumerateArray(), p => p.GetProperty("name").GetString() == "Mouse");
    }

    [Fact]
    public async Task Delete_on_an_id_that_never_existed_returns_404()
    {
        var response = await Client.DeleteAsync("/api/products/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_on_an_id_that_never_existed_returns_404()
    {
        var response = await Client.GetAsync("/api/products/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
