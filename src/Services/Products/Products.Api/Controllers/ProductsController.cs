using Products.Api.Dtos;

namespace Products.Api.Controllers;

// Empty on purpose: write the actions yourself, one exercise at a time.
// Exercises on this route (doc/aspnet-mock-interview-exercises.md):
//   POST-2, POST-3, GET-1, GET-2, GET-3, GET-5, PUT-1, PUT-4, PUT-5,
//   PATCH-4, PATCH-5, DELETE-1, DELETE-4, DELETE-5
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    [HttpPost]
    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> AddProduct([FromBody] ProductDto product)
    {
        var newProduct = new Product() 
        {
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Sku = product.Sku,
            Stock = product.Stock            
        };

        await _db.Products.AddAsync(newProduct);
        await _db.SaveChangesAsync();

        // Original (bug): nameof(newProduct.Id) is the string "Id". The first argument must be the
        // NAME OF THE GET ACTION, not a property. No action is called "Id", so building the
        // Location header threw "No route matches the supplied values" -> 500.
        // return CreatedAtAction(nameof(newProduct.Id), new { id = newProduct.Id }, newProduct);

        // Fix: point at the GetById action; the anonymous object fills its {id} route value.
           return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct); // Option A
        // return CreatedAtRoute("GetProductById", new { id = newProduct.Id }, newProduct); // Option B
        // return Created($"/api/products/{newProduct.Id}", newProduct); // Option C

        // All three return 201 Created + a Location header + the body. They differ only in
        // HOW the Location URL is built:
        //
        // 1) CreatedAtAction(actionName, routeValues, body)
        //    Finds the URL by ACTION NAME (the C# method), in this controller by default.
        //      return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        //    Use when the GET lives in the same controller. Refactor-safe thanks to nameof.
        //    Trap: an action named GetByIdAsync fails at runtime, because MVC strips the
        //    "Async" suffix from action names (SuppressAsyncSuffixInActionNames = true).
        //
        // 2) CreatedAtRoute(routeName, routeValues, body)
        //    Finds the URL by ROUTE NAME, set with [HttpGet("{id:int}", Name = "GetProductById")].
        //      return CreatedAtRoute("GetProductById", new { id = newProduct.Id }, newProduct);
        //    Use when the GET is in another controller, or you want to rename methods freely.
        //    The name is a string that must be unique in the whole app.
        //
        // 3) Created(uri, body)
        //    No lookup at all: YOU write the URL as a string.
        //      return Created($"/api/products/{newProduct.Id}", newProduct);
        //    Use when the resource URL is outside this app (another service) or there is no
        //    GET action. Nothing checks the string, so a typo gives a broken Location.
    }

    [HttpGet("{id:int}", Name = "GetProductById")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return product is null ? NotFound() : product;
    }
}
