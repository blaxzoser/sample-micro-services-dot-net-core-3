using System.Net;
using System.Text.Json;

namespace Products.Api.Tests;

// GET-2 · Filter by category → 200 (with an empty list)
[Trait("Exercise", "GET-2")]
public class Get2_FilterByCategoryTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Theory]
    [InlineData("/api/products", 3)]
    [InlineData("/api/products?category=electronics", 2)]   // case-insensitive
    [InlineData("/api/products?category=FURNITURE", 1)]
    public async Task Returns_200_with_the_matching_products(string url, int expectedCount)
    {
        var response = await Client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
        Assert.Equal(expectedCount, body.GetArrayLength());
    }

    [Fact]
    public async Task Unknown_category_returns_200_with_an_empty_array_not_404()
    {
        var response = await Client.GetAsync("/api/products?category=toys");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
        Assert.Equal(0, body.GetArrayLength());
    }

    [Fact]
    public async Task Items_are_DTOs_with_only_id_name_category_and_price()
    {
        var response = await Client.GetAsync("/api/products?category=Furniture");

        var item = (await ReadJsonAsync(response))[0];
        Assert.Equal(3, item.GetProperty("id").GetInt32());
        Assert.Equal("Desk", item.GetProperty("name").GetString());
        Assert.Equal("Furniture", item.GetProperty("category").GetString());
        Assert.Equal(300m, item.GetProperty("price").GetDecimal());

        Assert.False(HasProperty(item, "sku"), "The DTO must not expose sku.");
        Assert.False(HasProperty(item, "stock"), "The DTO must not expose stock.");
        Assert.False(HasProperty(item, "version"), "The DTO must not expose version.");
        Assert.False(HasProperty(item, "isDeleted"), "The DTO must not expose isDeleted.");
    }

    [Fact]
    public async Task Soft_deleted_products_are_left_out()
    {
        Factory.MarkDeleted(2);

        var response = await Client.GetAsync("/api/products");

        var body = await ReadJsonAsync(response);
        Assert.Equal(2, body.GetArrayLength());
        Assert.DoesNotContain(body.EnumerateArray(), p => p.GetProperty("id").GetInt32() == 2);
    }
}
