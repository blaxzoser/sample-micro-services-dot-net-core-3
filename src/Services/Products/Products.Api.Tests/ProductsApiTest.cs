using System.Net.Http.Json;
using System.Text.Json;

namespace Products.Api.Tests;

/// <summary>
/// Base class for the exercise tests. xUnit creates a new instance for every test, so the
/// constructor gives each test fresh seed data: Laptop (1), Mouse (2), Desk (3).
/// </summary>
public abstract class ProductsApiTest : IClassFixture<ProductsApiFactory>
{
    protected readonly ProductsApiFactory Factory;
    protected readonly HttpClient Client;

    protected ProductsApiTest(ProductsApiFactory factory)
    {
        Factory = factory;
        Factory.ResetDatabase();
        Client = factory.CreateClient();
    }

    /// <summary>A valid body for POST /api/products and PUT /api/products/{id}.</summary>
    protected static object ProductBody(
        string name = "Chair", string sku = "CHR-001", string category = "Furniture",
        decimal price = 80m, int stock = 3)
        => new { name, sku, category, price, stock };

    protected static JsonContent Json(object body) => JsonContent.Create(body);

    protected static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(text), "Expected a JSON body, but the response body is empty.");
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    /// <summary>Sends a request with one extra header, such as If-Match or X-Api-Key.</summary>
    protected Task<HttpResponseMessage> SendAsync(
        HttpMethod method, string url, string? header = null, string? headerValue = null, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, url) { Content = content };
        if (header is not null)
            request.Headers.TryAddWithoutValidation(header, headerValue);
        return Client.SendAsync(request);
    }

    /// <summary>True if the object has the property, ignoring case (ValidationProblem keys vary: "Sku" or "sku").</summary>
    protected static bool HasProperty(JsonElement obj, string name)
        => obj.ValueKind == JsonValueKind.Object &&
           obj.EnumerateObject().Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
}
