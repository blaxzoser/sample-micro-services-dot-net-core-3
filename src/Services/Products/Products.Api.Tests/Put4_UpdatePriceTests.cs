using System.Net;

namespace Products.Api.Tests;

// PUT-4 · Update price: validation (400) vs business rule (422)
// Laptop costs 1200, so the highest allowed new price is 1800.
[Trait("Exercise", "PUT-4")]
public class Put4_UpdatePriceTests(ProductsApiFactory factory) : ProductsApiTest(factory)
{
    [Theory]
    [InlineData(1500)]
    [InlineData(1800)]   // the 50% limit is inclusive
    [InlineData(100)]    // lowering the price is always fine
    public async Task Allowed_price_returns_204_and_saves_it(int price)
    {
        var response = await Client.PutAsync("/api/products/1/price", Json(new { price }));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(price, Factory.FindProduct(1)!.Price);
    }

    [Fact]
    public async Task More_than_50_percent_above_returns_422_with_the_limits_in_the_extensions()
    {
        var response = await Client.PutAsync("/api/products/1/price", Json(new { price = 2000 }));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(1200m, body.GetProperty("currentPrice").GetDecimal());
        Assert.Equal(1800m, body.GetProperty("maxAllowedPrice").GetDecimal());
    }

    [Fact]
    public async Task Rejected_price_is_not_saved()
    {
        await Client.PutAsync("/api/products/1/price", Json(new { price = 2000 }));

        Assert.Equal(1200m, Factory.FindProduct(1)!.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Zero_or_negative_price_returns_400(int price)
    {
        var response = await Client.PutAsync("/api/products/1/price", Json(new { price }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(1200m, Factory.FindProduct(1)!.Price);
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await Client.PutAsync("/api/products/999/price", Json(new { price = 10 }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
