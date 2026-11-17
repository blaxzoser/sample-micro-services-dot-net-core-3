namespace Products.Api.Models;

// Entity from the starter kit in doc/aspnet-mock-interview-exercises.md.
// It has no navigation properties on purpose, so the JSON matches the exercise test cases.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Sku { get; set; } = "";       // unique in the database (POST-3)
    public string Category { get; set; } = "";  // GET-2
    public decimal Price { get; set; }          // PUT-4
    public int Stock { get; set; }              // PATCH-5
    public bool IsDeleted { get; set; }         // DELETE-4 (soft delete)
    public int Version { get; set; } = 1;
}
