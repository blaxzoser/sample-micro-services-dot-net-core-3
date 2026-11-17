using System.Net;

namespace Products.Api.Tests;

// GET-3 · Pagination + X-Total-Count → 200 / 400
[Trait("Exercise", "GET-3")]
public class Get3_PaginationTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task First_page_returns_two_items_the_totals_and_the_X_Total_Count_header()
    {
        var response = await Client.GetAsync("/api/products/paged?page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Total-Count", out var header), "Missing X-Total-Count header.");
        Assert.Equal("3", header!.Single());

        var body = await ReadJsonAsync(response);
        var ids = body.GetProperty("items").EnumerateArray().Select(p => p.GetProperty("id").GetInt32()).ToArray();
        Assert.Equal(new[] { 1, 2 }, ids);
        Assert.Equal(1, body.GetProperty("page").GetInt32());
        Assert.Equal(2, body.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, body.GetProperty("totalCount").GetInt32());
        Assert.Equal(2, body.GetProperty("totalPages").GetInt32());
    }

    [Fact]
    public async Task Second_page_returns_the_last_item()
    {
        var response = await Client.GetAsync("/api/products/paged?page=2&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        var ids = body.GetProperty("items").EnumerateArray().Select(p => p.GetProperty("id").GetInt32()).ToArray();
        Assert.Equal(new[] { 3 }, ids);
    }

    [Fact]
    public async Task Page_past_the_end_returns_200_with_empty_items()
    {
        var response = await Client.GetAsync("/api/products/paged?page=5&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(0, body.GetProperty("items").GetArrayLength());
        Assert.Equal(3, body.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task Defaults_are_page_1_and_pageSize_10()
    {
        var response = await Client.GetAsync("/api/products/paged");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(1, body.GetProperty("page").GetInt32());
        Assert.Equal(10, body.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, body.GetProperty("items").GetArrayLength());
        Assert.Equal(1, body.GetProperty("totalPages").GetInt32());
    }

    [Theory]
    [InlineData("?page=0")]
    [InlineData("?page=-1")]
    [InlineData("?pageSize=0")]
    [InlineData("?pageSize=51")]
    [InlineData("?pageSize=100")]
    [InlineData("?page=abc")]
    public async Task Invalid_paging_values_return_400(string query)
    {
        var response = await Client.GetAsync("/api/products/paged" + query);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PageSize_50_is_allowed()
    {
        var response = await Client.GetAsync("/api/products/paged?pageSize=50");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
