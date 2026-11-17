using Products.Api.Dtos;

namespace Products.Api.Controllers;

// Empty on purpose: this is exercise POST-7 (doc/aspnet-mock-interview-exercises.md).
// Same "create product" as POST-2, but stored in a List<T> instead of SQL Server, so YOU
// must generate the id (NextId) and protect the list from simultaneous requests (lock).
//
// Write three actions:
//   POST /api/memory/products        -> 201 + Location + body (reuse ProductDto for validation)
//   GET  /api/memory/products/{id}   -> 200 / 404
//   GET  /api/memory/products        -> 200 with every product
//
// Run the tests:  dotnet test Products.Api.Tests --filter "Exercise=POST-7"
[ApiController]
[Route("api/memory/products")]
public class MemoryProductsController : ControllerBase
{
    private readonly InMemoryProductStore _store;

    public MemoryProductsController(InMemoryProductStore store) => _store = store;
}
