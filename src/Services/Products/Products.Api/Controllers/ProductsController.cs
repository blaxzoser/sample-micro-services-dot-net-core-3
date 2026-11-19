using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Dtos;
using Products.Api.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Products.Api.Controllers;

// Empty on purpose: write the actions yourself, one exercise at a time.
// Exercises on this route (doc/aspnet-mock-interview-exercises.md):
//   POST-2, POST-3, GET-1, GET-2, GET-3, GET-5, PUT-1, PUT-4, PUT-5,
//   PATCH-4 (partial update), PATCH-5, DELETE-1, DELETE-4, DELETE-5
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
        return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct); // Option A
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

    //[HttpGet("{id:int}", Name = "GetProductById")]
    //public ActionResult<Product> GetById(int id)
    //{
    //    var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
    //    return product is null ? NotFound() : product;
    //}

    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [HttpPost("withUnique")]
    public async Task<ActionResult<Product>> AddProductUnique([FromBody] ProductDto product)
    {

        var isExistsProductSku = _db.Products.AsNoTracking().Any(s => s.Sku == product.Sku && !s.IsDeleted);
        if (isExistsProductSku)
            return Problem(statusCode: StatusCodes.Status409Conflict,
                          title: "Duplicate SKU",
                          detail: $"sku is {product.Sku} exist.");

        var newProduct = new Product()
        {
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Sku = product.Sku,
            Stock = product.Stock
        };

        await _db.Products.AddAsync(newProduct);

        try {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)   // unique index violated by a concurrent request
        {
            return Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Duplicate SKU ",
                    detail: $"sku is {product.Sku} exist.");
        }

        return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> Get([FromRoute] int id)
    {
        var product = await _db.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        return product is null ? Problem(detail: "Non found", statusCode: StatusCodes.Status404NotFound) : product;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<List<RecordProductDto>>> GetByCategoryAsync([FromQuery] string? category)
    {
        var allProducts = _db.Products.AsNoTracking().Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(category))
        {
            allProducts = allProducts.Where(p => p.Category.ToLower() == category.ToLower());
        }

        return await allProducts
        .Select(p => new RecordProductDto(p.Id, p.Name, p.Category, p.Price)).ToListAsync();
        // if it was empty , it will return automatiy a empty list
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("paged")]
    public async Task<ActionResult<Pagination<List<ProductDto>>>> GetByPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 50)
            return Problem(statusCode: StatusCodes.Status400BadRequest);

        var products = _db.Products.AsNoTracking().Where(p => !p.IsDeleted).OrderBy(p => p.Id);     // its not create a new query , its a defered        
        var totalProducts = await products.CountAsync(); // this create a new query
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

        if (totalPages < page)
        {
            Response.Headers["X-Total-Count"] = totalProducts.ToString();
            return Ok(new Pagination<List<ProductDto>> { Items = new List<ProductDto>(), TotalCount = totalProducts, Page = page, PageSize = pageSize, TotalPages = totalPages });
        }

        var filterProducts = await products.
            Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto() { Category = p.Category, Id = p.Id, Name = p.Name, Price = p.Price, Sku = p.Sku, Stock = p.Stock })
            .ToListAsync();  // this create a new query, we have only 2 

        var newPagination = new Pagination<List<ProductDto>>();
        newPagination.Items = filterProducts;
        newPagination.PageSize = pageSize;
        newPagination.Page = page;
        newPagination.TotalPages = totalPages;
        newPagination.TotalCount = totalProducts;

        Response.Headers["X-Total-Count"] = totalProducts.ToString();
        return newPagination;
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateWithPut([FromRoute]int id,[FromBody] ProductDto productDto)
    {
        var product = _db.Products.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefault();
        if(product == null)
        {
            return NotFound();         
        }

        product.Name = productDto.Name;
        product.Price = productDto.Price;  
        product.Sku = productDto.Sku;
        product.Stock = productDto.Stock;
        product.Category = productDto.Category;
        product.Version++;

        await _db.SaveChangesAsync();

        return NoContent(); 
    }


    [ProducesResponseType(statusCode: StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode:StatusCodes.Status200OK)]
    [HttpPatch("{id:int}")]
    public ActionResult<Product> UpdateByPatch(int id,[FromBody] PatchUserRequest productDto) { 
   
        if (productDto.Name is null & productDto.Price is null & productDto.Stock is null & productDto.Category is null)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No fields to update");


        var product = _db.Products.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefault();
        if (product == null)
            return NotFound();


        if (productDto.Name is not null)
                product.Name = productDto.Name;

        if (productDto.Price is not null)
            product.Price = productDto.Price.Value;

        if (productDto.Stock is not null)
            product.Stock = productDto.Stock.Value;

        if (productDto.Category is not null) 
            product.Category = productDto.Category;

        product.Version++;
        _db.SaveChanges();

        return Ok(product);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id) {
        var product = _db.Products.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefault();
        
        if(product == null) return Problem(statusCode:StatusCodes.Status404NotFound);

        _db.Products.Remove(product);
        _db.SaveChanges();

        return NoContent();
    }

}

