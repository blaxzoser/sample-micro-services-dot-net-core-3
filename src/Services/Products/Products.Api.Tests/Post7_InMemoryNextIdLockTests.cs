using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Products.Api.Tests;

// POST-7 · In-memory create: NextId + lock → 201 Created / 400
//
// These tests do NOT use SQL Server. xUnit creates a new instance of this class for every
// test, and each instance starts its own copy of the app, so the singleton
// InMemoryProductStore always begins with the three seed products (next id = 4).
[Trait("Exercise", "POST-7")]
public class Post7_InMemoryNextIdLockTests : IDisposable
{
    private const string Url = "/api/memory/products";

    private readonly MemoryApiFactory _factory = new();
    private readonly HttpClient _client;

    public Post7_InMemoryNextIdLockTests() => _client = _factory.CreateClient();

    public void Dispose() => _factory.Dispose();

    // ---------- NextId ----------

    [Fact]
    public async Task First_product_returns_201_with_id_4_and_a_Location_header()
    {
        var response = await PostAsync();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith($"{Url}/4", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);

        var body = await ReadJsonAsync(response);
        Assert.Equal(4, body.GetProperty("id").GetInt32());
        Assert.Equal("Chair", body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Ids_increase_by_one_on_every_create()
    {
        var ids = new List<int>();
        for (var i = 0; i < 3; i++)
            ids.Add(await IdOf(await PostAsync()));

        Assert.Equal(new[] { 4, 5, 6 }, ids);
    }

    [Fact]
    public async Task Location_url_resolves_to_the_new_product()
    {
        var created = await PostAsync();
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        // A second request sees the product, so the store must be shared (singleton).
        var response = await _client.GetAsync(created.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Chair", (await ReadJsonAsync(response)).GetProperty("name").GetString());
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await _client.GetAsync($"{Url}/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_all_returns_the_three_seed_products_plus_new_ones()
    {
        Assert.Equal(3, await CountAsync());

        await PostAsync();

        Assert.Equal(4, await CountAsync());
    }

    [Fact]
    public async Task Invalid_product_returns_400_and_does_not_use_up_an_id()
    {
        var invalid = await PostAsync(price: 0m);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal(3, await CountAsync());

        // The rejected request must not have consumed id 4.
        Assert.Equal(4, await IdOf(await PostAsync()));
    }

    // ---------- lock ----------

    // Sends many requests AT THE SAME TIME. Without a lock, two requests read the same
    // "highest id" and both create the same id, or List<T>.Add loses/corrupts items.
    private const int Rounds = 5;
    private const int RequestsPerRound = 100;
    private const int Total = Rounds * RequestsPerRound;

    [Fact]
    public async Task Parallel_creates_all_return_201()
    {
        var responses = await PostInParallelAsync();

        var failed = responses.Where(r => r.StatusCode != HttpStatusCode.Created).ToList();
        Assert.True(failed.Count == 0,
            $"{failed.Count} of {Total} parallel requests did not return 201 (first: {(int?)failed.FirstOrDefault()?.StatusCode}). " +
            "List<T> is not thread-safe: lock around NextId + Add.");
    }

    [Fact]
    public async Task Parallel_creates_never_reuse_an_id()
    {
        var ids = await IdsOf(await PostInParallelAsync());

        var duplicates = ids.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).OrderBy(id => id).ToList();
        Assert.True(duplicates.Count == 0,
            $"{duplicates.Count} ids were handed out more than once (for example {string.Join(", ", duplicates.Take(5))}). " +
            "Two requests read the same highest id: 'compute the id' and 'add' must be inside ONE lock.");
    }

    [Fact]
    public async Task Parallel_creates_use_exactly_the_ids_4_to_503()
    {
        var ids = await IdsOf(await PostInParallelAsync());

        Assert.Equal(Enumerable.Range(4, Total), ids.OrderBy(id => id));
    }

    [Fact]
    public async Task Parallel_creates_are_all_stored()
    {
        await PostInParallelAsync();

        // No lost writes: 3 seed products + every created one.
        Assert.Equal(3 + Total, await CountAsync());
    }

    // ---------- helpers ----------

    private Task<HttpResponseMessage> PostAsync(decimal price = 80m)
        => _client.PostAsync(Url, JsonContent.Create(
            new { name = "Chair", sku = "CHR-001", category = "Furniture", price, stock = 3 }));

    private async Task<List<HttpResponseMessage>> PostInParallelAsync()
    {
        var all = new List<HttpResponseMessage>();
        for (var round = 0; round < Rounds; round++)
        {
            // Task.Run puts every request on the thread pool, so they really overlap.
            var tasks = Enumerable.Range(0, RequestsPerRound).Select(_ => Task.Run(() => PostAsync()));
            all.AddRange(await Task.WhenAll(tasks));
        }
        return all;
    }

    private static async Task<List<int>> IdsOf(IEnumerable<HttpResponseMessage> responses)
    {
        var ids = new List<int>();
        foreach (var response in responses.Where(r => r.StatusCode == HttpStatusCode.Created))
            ids.Add(await IdOf(response));
        return ids;
    }

    private static async Task<int> IdOf(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).GetProperty("id").GetInt32();
    }

    private async Task<int> CountAsync()
    {
        var response = await _client.GetAsync(Url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).GetArrayLength();
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(text), "Expected a JSON body, but the response body is empty.");
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    /// <summary>The app with no database work: "Testing" skips the EnsureDeleted/EnsureCreated block in Program.cs.</summary>
    private sealed class MemoryApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.UseEnvironment("Testing");
    }
}
