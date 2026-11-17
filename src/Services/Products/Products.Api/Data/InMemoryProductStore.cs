namespace Products.Api.Data;

// POST-7 starter: an in-memory "database" (a plain List<T>), registered as a SINGLETON in
// Program.cs, so every request shares this one instance. It resets when the app restarts.
//
// Unlike SQL Server, a List<T> gives you nothing for free:
//   - no identity column  -> you must generate the next id yourself
//   - not thread-safe     -> two requests at the same time can corrupt it or reuse an id
//
// TODO (POST-7): add the two members the exercise asks for:
//   1) a lock object, e.g.  public readonly object Sync = new();
//   2) NextId(), which returns 1 for an empty list, or the highest id + 1
public class InMemoryProductStore
{

    public readonly object Sync = new();

    // Same seed data as AppDbContext, so the next new id is 4.
    public List<Product> Products { get; } = new()
    {
        new() { Id = 1, Name = "Laptop", Sku = "LAP-001", Category = "Electronics", Price = 1200m, Stock = 10 },
        new() { Id = 2, Name = "Mouse",  Sku = "MOU-001", Category = "Electronics", Price = 25m,   Stock = 100 },
        new() { Id = 3, Name = "Desk",   Sku = "DSK-001", Category = "Furniture",   Price = 300m,  Stock = 5 },
    };

    public int NextId() => Products.Count == 0 ? 1 : Products.Max(p => p.Id) + 1;
}
