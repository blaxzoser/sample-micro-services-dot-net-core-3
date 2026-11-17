using System.Net;

namespace Products.Api.Tests;

// POST-2 · Create product → 201 Created / 400
[Trait("Exercise", "POST-2")]
public class Post2_CreateProductTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Fact]
    public async Task Valid_product_returns_201_with_id_4_and_a_Location_header()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith("/api/products/4", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);

        var body = await ReadJsonAsync(response);
        Assert.Equal(4, body.GetProperty("id").GetInt32());
        Assert.Equal("Chair", body.GetProperty("name").GetString());
        Assert.Equal("CHR-001", body.GetProperty("sku").GetString());
        Assert.Equal("Furniture", body.GetProperty("category").GetString());
        Assert.Equal(80m, body.GetProperty("price").GetDecimal());
        Assert.Equal(3, body.GetProperty("stock").GetInt32());
    }

    [Fact]
    public async Task Valid_product_is_saved_in_the_database()
    {
        await Client.PostAsync("/api/products", Json(ProductBody()));

        var saved = Factory.FindProduct(4);
        Assert.NotNull(saved);
        Assert.Equal("Chair", saved!.Name);
        Assert.Equal("CHR-001", saved.Sku);
        Assert.Equal(80m, saved.Price);
        Assert.Equal(3, saved.Stock);
    }

    [Fact]
    public async Task Location_url_resolves_to_the_new_product()
    {
        var created = await Client.PostAsync("/api/products", Json(ProductBody()));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var response = await Client.GetAsync(created.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("Chair", body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Sku_with_wrong_format_returns_400_with_errors_Sku()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(sku: "chair1")));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.True(HasProperty(body.GetProperty("errors"), "Sku"), "Expected errors.Sku in the ValidationProblem body.");
    }

    [Fact]
    public async Task Price_zero_returns_400_with_errors_Price()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(price: 0m)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.True(HasProperty(body.GetProperty("errors"), "Price"), "Expected errors.Price in the ValidationProblem body.");
    }

    [Fact]
    public async Task Negative_stock_returns_400()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(stock: -1)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Empty_name_returns_400()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(name: "")));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Name_longer_than_100_characters_returns_400()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(name: new string('a', 101))));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Empty_category_returns_400()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(category: "")));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Price_above_one_million_returns_400()
    {
        var response = await Client.PostAsync("/api/products", Json(ProductBody(price: 1_000_000.01m)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_product_is_not_saved()
    {
        await Client.PostAsync("/api/products", Json(ProductBody(price: 0m)));

        Assert.Null(Factory.FindProduct(4));
    }
}
