using Products.Api.Dtos;
using Products.Api.Models;

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

    [HttpPost]
    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> AddProduct([FromBody] ProductDto product)
    {
        // REVIEW FIX: build the entity OUTSIDE the lock. Mapping the DTO touches no shared
        // state, so it does not need protection. Keep the lock as short as possible.
        var newProduct = new Product
        {
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Sku = product.Sku,
            Stock = product.Stock,
        };

        // Only the two lines that touch the shared list are locked: "read the highest id"
        // and "add" must be ONE atomic step, or two requests get the same id.
        lock (_store.Sync)
        {
            newProduct.Id = _store.NextId();
            _store.Products.Add(newProduct);
        }

        // REVIEW FIX: CreatedAtAction builds the Location header from the route of
        // GetProductById, instead of a hardcoded string that breaks if the route changes.
        // (It was also returned from inside the lock; the response is now built outside.)
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
    }

    // REVIEW FIX (the bug that failed the test "Location_url_resolves_to_the_new_product"):
    // this was `[FromQuery] int id`. The id is in the PATH (/api/memory/products/4), not in
    // the query string (?id=4), so [FromQuery] found nothing, id stayed 0, and every call
    // returned 404. A parameter with the same name as the {id} route token binds from the
    // route by default; [FromRoute] only makes that explicit.
    [HttpGet("{id:int}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetProductById([FromRoute] int id)
    {
        // REVIEW FIX: only the read of the list is locked; the response is built outside.
        // Renamed `isExistProduct` to `product`: "is..." reads like a bool, but it is a Product.
        Product? product;
        lock (_store.Sync) product = _store.Products.FirstOrDefault(p => p.Id == id);

        if (product is null)
            return Problem(statusCode: StatusCodes.Status404NotFound,
                           title: "Product not found",
                           detail: $"No product with id {id} exists.");   // clearer wording

        return Ok(product);
    }

    [HttpGet]
    [ProducesResponseType<List<Product>>(StatusCodes.Status200OK)]
    public ActionResult<List<Product>> GetAllProducts()
    {
        // Correct as you wrote it: ToList() copies inside the lock, so the serializer never
        // enumerates the live list while another request is adding to it.
        lock (_store.Sync) return _store.Products.ToList();
    }



}
