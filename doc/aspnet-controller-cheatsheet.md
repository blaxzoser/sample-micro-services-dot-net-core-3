# ASP.NET Core Controller Cheat Sheet (Senior Interview)

A quick reference for building REST controllers: HTTP verbs, routes, status codes, and the `ControllerBase` helper for each one.
The examples follow this repo: `Catalog.Api` with **CQRS + MediatR**, `ProductController` (`/products`), and `ProductInStockController` (`/stocks`).

> Repo target: `netcoreapp3.1`. Anything that needs .NET 6+ is marked **(.NET 6+)**.

---

## 1. HTTP verbs at a glance

| Verb | Purpose | Idempotent? | Safe? | Body? | Typical success | Typical errors |
|---|---|---|---|---|---|---|
| `GET` | Read a resource or a collection | ✅ | ✅ | No | `200 OK` (`304` with caching) | `404`, `400` |
| `POST` | Create, or run a command/action | ❌ | ❌ | Yes | `201 Created` + `Location` header, `202 Accepted`, `200` | `400`, `409`, `422` |
| `PUT` | Replace the whole resource (or upsert) | ✅ | ❌ | Yes | `204 No Content`, `200 OK` (return body), `201` (upsert created it) | `400`, `404`, `409`, `412` |
| `PATCH` | Partial update | ❌ (usually) | ❌ | Yes | `204` or `200` | `400`, `404`, `409`, `422` |
| `DELETE` | Remove | ✅ | ❌ | Rarely | `204 No Content` (`200` with body, `202` if async) | `404`*, `409` |
| `HEAD` | Same as GET, headers only | ✅ | ✅ | No | `200` | `404` |
| `OPTIONS` | Allowed methods, CORS preflight | ✅ | ✅ | No | `204`/`200` | — |

\* For an idempotent DELETE, many teams return `204` even when the resource is already gone. Choose one rule and keep it consistent.

**Interview one-liners**
- **Idempotent** means calling it N times leaves the server in the same state as calling it once. PUT and DELETE are idempotent; POST is not.
- **Safe** means it doesn't change server state (GET, HEAD, OPTIONS).
- PUT = "here is the full new version". PATCH = "here are the changes".

---

## 2. Status codes and their `ControllerBase` helpers

### 2xx Success
| Code | Meaning | Helper | When |
|---|---|---|---|
| 200 | OK | `Ok()` / `Ok(obj)` / `return obj;` | GET, or an update that returns a body |
| 201 | Created | `CreatedAtAction(...)`, `CreatedAtRoute(...)`, `Created(uri, obj)` | POST created a resource |
| 202 | Accepted | `Accepted()`, `AcceptedAtAction(...)` | Queued or async work (message bus, background job) |
| 204 | No Content | `NoContent()` | PUT, PATCH, or DELETE with nothing to return |

### 3xx Redirection
| Code | Meaning | Helper | Notes |
|---|---|---|---|
| 301 | Moved Permanently | `RedirectPermanent(url)` | The browser may change POST into GET |
| 302 | Found | `Redirect(url)`, `RedirectToAction(...)`, `LocalRedirect(url)` | Temporary. `LocalRedirect` blocks open-redirect attacks |
| 303 | See Other | `StatusCode(303)` + `Location` header | "Go GET the result over there" (after a POST) |
| 304 | Not Modified | `StatusCode(304)` | ETag / If-None-Match caching |
| 307 | Temporary Redirect | `RedirectPreserveMethod(url)` | Keeps the method and body |
| 308 | Permanent Redirect | `RedirectPermanentPreserveMethod(url)` | Keeps the method and body |

### 4xx Client errors
| Code | Meaning | Helper | When |
|---|---|---|---|
| 400 | Bad Request | `BadRequest()`, `BadRequest(ModelState)`, `ValidationProblem()` | Bad input or failed validation (sent automatically by `[ApiController]`) |
| 401 | Unauthorized | `Unauthorized()` | Not authenticated: no token, or the token is invalid |
| 403 | Forbidden | `Forbid()` | Authenticated but not allowed |
| 404 | Not Found | `NotFound()` | The resource doesn't exist |
| 405 | Method Not Allowed | (framework) | Verb isn't mapped |
| 409 | Conflict | `Conflict()` | Duplicate key, concurrency clash, business-state conflict |
| 412 | Precondition Failed | `StatusCode(412)` | `If-Match` ETag mismatch (optimistic concurrency) |
| 415 | Unsupported Media Type | (framework) | Wrong `Content-Type` |
| 422 | Unprocessable Entity | `UnprocessableEntity()` | Valid syntax, but a business rule fails (e.g., not enough stock) |
| 429 | Too Many Requests | `StatusCode(429)` | Rate limiting |

### 5xx Server errors
| Code | Meaning | Helper | When |
|---|---|---|---|
| 500 | Internal Server Error | `Problem()` / `StatusCode(500)` | Unhandled exception. **Never** leak the stack trace in prod |
| 502 / 503 / 504 | Bad gateway / unavailable / timeout | `StatusCode(503)` | Downstream service failures (common between microservices) |

Generic fallback: `StatusCode(StatusCodes.Status418ImATeapot, obj)` using `Microsoft.AspNetCore.Http.StatusCodes`.

---

## 3. Routing cheat sheet

```csharp
[ApiController]                 // automatic 400, binding inference, ProblemDetails
[Route("api/[controller]")]     // [controller] => "Products" (class name minus "Controller")
public class ProductsController : ControllerBase { }
```

| Attribute | Route | Example URL |
|---|---|---|
| `[HttpGet]` | `api/products` | `GET /api/products?page=1&take=10` |
| `[HttpGet("{id:int}")]` | `api/products/{id}` | `GET /api/products/5` |
| `[HttpGet("{id:int}/stock")]` | nested | `GET /api/products/5/stock` |
| `[HttpGet("search")]` | literal segment | `GET /api/products/search?name=tv` |
| `[HttpGet("~/health")]` | `~` ignores the controller prefix | `GET /health` |
| `[HttpGet("{slug:alpha:minlength(3)}")]` | stacked constraints | `GET /api/products/laptop` |
| `[HttpGet("{*path}")]` | catch-all | `GET /api/products/a/b/c` |
| `[HttpGet("{id:int}", Name = "GetProduct")]` | named route for `CreatedAtRoute` | — |

**Route constraints:** `int`, `long`, `guid`, `bool`, `datetime`, `decimal`, `alpha`, `min(1)`, `max(100)`, `range(1,100)`, `length(5)`, `minlength(3)`, `regex(^\\d{{3}}$)`, `required`. Optional parameter: `{id?}`. Default value: `{page=1}`.

> If a constraint fails, the route doesn't match, so the client gets a **404**, not a 400.

### Binding sources
```csharp
public IActionResult Example(
    [FromRoute]  int id,                  // /products/{id}
    [FromQuery]  int page = 1,            // ?page=2
    [FromQuery(Name = "q")] string term,  // ?q=tv
    [FromBody]   ProductCreateCommand cmd,// JSON body (only ONE FromBody per action)
    [FromHeader(Name = "X-Correlation-Id")] string correlationId,
    [FromForm]   IFormFile file,          // multipart/form-data
    [FromServices] ILogger<ProductsController> log) // DI injected into the method
```
With `[ApiController]`, complex types default to `[FromBody]`, and simple types default to the route if the name matches, otherwise the query string.

### Bind a comma-separated list (this repo's `?ids=1,2,3` pattern)
```csharp
// Repo version: string ids + Split + Convert.ToInt32 -> "1,abc" throws FormatException => 500
// Safer: let the binder handle it, so the client sends ?ids=1&ids=2&ids=3 and bad input gives an automatic 400
[HttpGet]
public Task<DataCollection<ProductDto>> GetAll([FromQuery] int[] ids, int page = 1, int take = 10)
    => _productQueryService.GetAllAsyc(page, take, ids.Length == 0 ? null : ids);
```

---

## 4. Return types: which to use

```csharp
// 1) Concrete type: always 200 (or 204 if null). Can't return 404. (The repo uses this today.)
public async Task<ProductDto> Get(int id)

// 2) IActionResult: any status, but Swagger can't infer the response schema
public async Task<IActionResult> Get(int id)

// 3) ActionResult<T>: RECOMMENDED. Return T (=> 200) OR NotFound()/BadRequest()...
public async Task<ActionResult<ProductDto>> Get(int id)
```

Document the responses for Swagger/OpenAPI:
```csharp
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```
Or add `[ApiConventionType(typeof(DefaultApiConventions))]` to the controller.

---

## 5. Full CRUD controller (CQRS + MediatR, the senior version)

```csharp
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProductQueryService _queries;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, IProductQueryService queries,
                              ILogger<ProductsController> logger)
        => (_mediator, _queries, _logger) = (mediator, queries, logger);

    // GET api/v1/products?page=1&take=10
    [HttpGet]
    [ProducesResponseType(typeof(DataCollection<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<DataCollection<ProductDto>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int take = 10, CancellationToken ct = default)
    {
        if (page < 1 || take is < 1 or > 100)          // guard (pattern syntax needs C# 9; on 3.1 use || )
            return BadRequest("page >= 1 and 1 <= take <= 100");

        return Ok(await _queries.GetAllAsyc(page, take));
    }

    // GET api/v1/products/5
    [HttpGet("{id:int}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
    {
        var product = await _queries.FindAsync(id);    // FirstOrDefault, NOT Single => no exception
        if (product is null) return NotFound();
        return product;                                // implicit 200
    }

    // POST api/v1/products  -> 201 + Location: /api/v1/products/{newId}
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductCreateCommand cmd, CancellationToken ct)
    {
        // With IRequest<int> + Send (instead of INotification + Publish) the handler can return the new id
        var newId = await _mediator.Send(cmd, ct);
        var dto = await _queries.FindAsync(newId);
        return CreatedAtAction(nameof(GetById), new { id = newId }, dto);
    }

    // PUT api/v1/products/5  -> replace the whole product
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateCommand cmd, CancellationToken ct)
    {
        if (id != cmd.ProductId) return BadRequest("Route id and body id don't match");

        var found = await _mediator.Send(cmd, ct);     // handler returns bool
        return found ? NoContent() : NotFound();
    }

    // PATCH api/v1/products/5   (Content-Type: application/json-patch+json)
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id,
        [FromBody] JsonPatchDocument<ProductUpdateDto> patch, CancellationToken ct)
    {
        var current = await _queries.FindForUpdateAsync(id);
        if (current is null) return NotFound();

        patch.ApplyTo(current, ModelState);            // pkg: Microsoft.AspNetCore.Mvc.NewtonsoftJson
        if (!TryValidateModel(current)) return ValidationProblem(ModelState);

        await _mediator.Send(new ProductUpdateCommand(id, current), ct);
        return NoContent();
    }

    // DELETE api/v1/products/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _mediator.Send(new ProductDeleteCommand(id), ct);
        return deleted ? NoContent() : NotFound();
    }
}
```

PATCH body example (RFC 6902):
```json
[
  { "op": "replace", "path": "/price", "value": 99.90 },
  { "op": "remove",  "path": "/description" }
]
```
PATCH setup: `services.AddControllers().AddNewtonsoftJson();`

### Command that returns a value (MediatR `IRequest<T>`)
```csharp
public class ProductCreateCommand : IRequest<int>       // the repo uses INotification (fire and forget)
{
    [Required, StringLength(100)] public string Name { get; set; }
    public string Description { get; set; }
    [Range(0.01, 999999)] public decimal Price { get; set; }
}

public class ProductCreateHandler : IRequestHandler<ProductCreateCommand, int>
{
    private readonly ApplicationDbContext _db;
    public ProductCreateHandler(ApplicationDbContext db) => _db = db;

    public async Task<int> Handle(ProductCreateCommand cmd, CancellationToken ct)
    {
        var entity = new Product { Name = cmd.Name, Description = cmd.Description, Price = cmd.Price };
        await _db.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity.ProductId;
    }
}
```
> **`Publish` vs `Send`:** `Publish(INotification)` goes to **0..N** handlers with no return value; use it for domain events. `Send(IRequest<T>)` goes to **exactly 1** handler and returns `T`; use it for commands and queries. A command that creates something should be `Send`, so you get the id back for the `201`.

---

## 6. Non-CRUD and "action" endpoints

```csharp
// Bulk/command endpoint, like the repo's PUT /stocks
[HttpPut]                                   // repo: PUT on the collection = bulk update
public async Task<IActionResult> UpdateStock([FromBody] ProductInStockUpdateCommnand cmd)
{
    await _mediator.Publish(cmd);
    return NoContent();                     // 204
}

// Action on a resource: POST + verb segment
[HttpPost("{id:int}/discontinue")]
public async Task<IActionResult> Discontinue(int id) { ... return NoContent(); }

// Long-running job -> 202 + where to poll
[HttpPost("reindex")]
public IActionResult Reindex()
{
    var jobId = _jobs.Enqueue();
    return AcceptedAtAction(nameof(GetJobStatus), new { jobId }, new { jobId });
}

[HttpGet("jobs/{jobId:guid}")]
public IActionResult GetJobStatus(Guid jobId) { ... }
```

### Redirect examples
```csharp
[HttpGet("old/{id:int}")]
public IActionResult Old(int id) => RedirectToActionPermanent(nameof(GetById), new { id }); // 301

[HttpGet("go")]
public IActionResult Go(string returnUrl) => LocalRedirect(returnUrl);  // 302, only to local URLs (safe)
```

### Conditional GET (304) with an ETag
```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id)
{
    var dto = await _queries.FindAsync(id);
    if (dto is null) return NotFound();

    var etag = $"\"{dto.RowVersion}\"";                     // or a hash of the payload
    if (Request.Headers["If-None-Match"] == etag) return StatusCode(StatusCodes.Status304NotModified);

    Response.Headers["ETag"] = etag;
    return Ok(dto);
}
```

### Optimistic concurrency (412 / 409)
```csharp
[HttpPut("{id:int}")]
public async Task<IActionResult> Update(int id, ProductUpdateCommand cmd,
                                        [FromHeader(Name = "If-Match")] string ifMatch)
{
    try { await _mediator.Send(cmd with { RowVersion = ifMatch }); }   // 'with' needs C# 9 records
    catch (DbUpdateConcurrencyException) { return StatusCode(StatusCodes.Status412PreconditionFailed); }
    return NoContent();
}
```

### File upload / download
```csharp
[HttpPost("{id:int}/image")]
[RequestSizeLimit(5_000_000)]
public async Task<IActionResult> Upload(int id, IFormFile file)
{
    if (file is null || file.Length == 0) return BadRequest("Empty file");
    if (!new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
        return StatusCode(StatusCodes.Status415UnsupportedMediaType);
    await using var stream = file.OpenReadStream();         // 'await using' needs C# 8
    ...
    return NoContent();
}

[HttpGet("{id:int}/image")]
public IActionResult Download(int id)
    => PhysicalFile(path, "image/png", "product.png");      // or File(bytes/stream, contentType, name)
```

---

## 7. Validation and errors (ProblemDetails)

### Automatic 400 with `[ApiController]`
If a DataAnnotation fails, the framework returns this **before your action runs**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "Name": ["The Name field is required."] }
}
```
Customize or turn it off:
```csharp
services.AddControllers()
    .ConfigureApiBehaviorOptions(o =>
    {
        // o.SuppressModelStateInvalidFilter = true;   // handle ModelState yourself
        o.InvalidModelStateResponseFactory = ctx =>
            new UnprocessableEntityObjectResult(new ValidationProblemDetails(ctx.ModelState));
    });
```
FluentValidation is common in senior codebases: register `AbstractValidator<T>` classes, or run them as a MediatR `IPipelineBehavior`.

### Manual errors
```csharp
ModelState.AddModelError(nameof(cmd.Price), "Price must be > 0");
return ValidationProblem(ModelState);                         // 400 ValidationProblemDetails

return Problem(title: "Not enough stock", detail: $"Product {id}",
               statusCode: StatusCodes.Status422UnprocessableEntity);
```

### Global exception handling: map domain exceptions to status codes
Throw typed exceptions from handlers instead of `new System.Exception(...)`, which is what `ProductInStockUpdateStockEventHandler` does today and becomes a 500.

```csharp
public class NotFoundException : Exception { public NotFoundException(string m) : base(m) {} }
public class BusinessRuleException : Exception { public BusinessRuleException(string m) : base(m) {} }

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _log;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> log) => (_next, _log) = (next, log);

    public async Task Invoke(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                NotFoundException      => (404, "Resource not found"),
                BusinessRuleException  => (422, ex.Message),
                DbUpdateConcurrencyException => (409, "Concurrency conflict"),
                UnauthorizedAccessException  => (403, "Forbidden"),
                _                      => (500, "Unexpected error")
            };
            if (status == 500) _log.LogError(ex, "Unhandled"); else _log.LogWarning(ex.Message);

            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsJsonAsync(new ProblemDetails   // WriteAsJsonAsync: .NET 5+; on 3.1 use JsonSerializer
            {
                Status = status, Title = title, Instance = ctx.Request.Path,
                Extensions = { ["traceId"] = ctx.TraceIdentifier }
            });
        }
    }
}
// Startup.Configure: register early, before UseRouting
app.UseMiddleware<ExceptionMiddleware>();
```
Other options: `app.UseExceptionHandler("/error")` with an `ErrorController`, an `IExceptionFilter`, or **(.NET 8+)** `IExceptionHandler` + `AddProblemDetails()`.

---

## 8. Auth status codes (401 vs 403)

```csharp
[Authorize]                                  // whole controller requires an authenticated user
public class OrdersController : ControllerBase
{
    [AllowAnonymous] [HttpGet] public ...    // public endpoint

    [Authorize(Roles = "Admin")]             // role-based
    [HttpDelete("{id:int}")] public ...

    [Authorize(Policy = "CanManageStock")]   // policy-based (preferred)
    [HttpPut] public ...
}
```
- **401**: who are you? (no token, or an expired one) → the client should log in.
- **403**: I know who you are, and you can't do this.
- Pipeline order matters: `UseRouting()` → `UseAuthentication()` → `UseAuthorization()` → `UseEndpoints()`. The repo's `Startup` has `UseAuthorization` but no `UseAuthentication` or auth scheme yet.

---

## 9. Pagination, filtering, and sorting

```csharp
// GET /products?page=2&take=20&sort=-price&name=tv
public record ProductQuery(int Page = 1, int Take = 10, string Sort = null, string Name = null);  // C# 9

[HttpGet]
public async Task<ActionResult<DataCollection<ProductDto>>> GetAll([FromQuery] ProductQuery q)
{
    var result = await _queries.SearchAsync(q);
    Response.Headers["X-Total-Count"] = result.Total.ToString();     // optional
    return Ok(result);   // the repo's DataCollection<T> already carries Items, Total, Page, Pages
}
```
Return an **empty list with 200** for a search with no matches, not a 404.

---

## 10. API versioning (microservices favorite)

```csharp
// pkg: Microsoft.AspNetCore.Mvc.Versioning (3.1) / Asp.Versioning.Mvc (.NET 6+)
services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;                 // api-supported-versions response header
});

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    [HttpGet, MapToApiVersion("2.0")]
    public IActionResult GetV2() => Ok();
}
```

---

## 11. Minimal API equivalents (.NET 6+), in case the interview uses them

```csharp
var g = app.MapGroup("/api/products").WithTags("Products");

g.MapGet("/", async (IProductQueryService q, int page = 1, int take = 10)
    => TypedResults.Ok(await q.GetAllAsyc(page, take)));

g.MapGet("/{id:int}", async Task<Results<Ok<ProductDto>, NotFound>> (int id, IProductQueryService q)
    => await q.FindAsync(id) is { } p ? TypedResults.Ok(p) : TypedResults.NotFound())
 .WithName("GetProduct");

g.MapPost("/", async (ProductCreateCommand cmd, IMediator m) =>
{
    var id = await m.Send(cmd);
    return TypedResults.CreatedAtRoute(new { id }, "GetProduct", new { id });
});

g.MapPut("/{id:int}", async (int id, ProductUpdateCommand cmd, IMediator m)
    => await m.Send(cmd) ? Results.NoContent() : Results.NotFound());

g.MapDelete("/{id:int}", async (int id, IMediator m)
    => await m.Send(new ProductDeleteCommand(id)) ? Results.NoContent() : Results.NotFound());
```

---

## 12. Code-review notes on this repo (good interview talking points)

| Where | Today | Senior fix |
|---|---|---|
| `ProductController.Get(int id)` | `SingleAsync` throws when the id is missing → **500** | `FirstOrDefaultAsync` + `return NotFound()` → **404**; return `ActionResult<ProductDto>` |
| `ProductController.Create` | `Publish` + `Ok()` → **200**, no id | `Send` (`IRequest<int>`) + `CreatedAtAction(...)` → **201** + `Location` |
| `[HttpGet("{id}")]` | No constraint, so `/products/abc` → 400 | `{id:int}` → 404 for non-numeric ids (clearer routing) |
| `GetAll(... string ids)` | `Convert.ToInt32` on bad input → **500** | `[FromQuery] int[] ids` → automatic 400 |
| `ProductInStockUpdateStockEventHandler` | `throw new System.Exception("not enough stock")` → **500** | `BusinessRuleException` mapped to **422**/**409** by middleware |
| Same handler | `entry.ProductId` is read when `entry == null` → `NullReferenceException` | Log `item.ProductId` instead |
| `ProductInStockController.ActionResult(...)` | Method named `ActionResult`, which is confusing | Rename it `UpdateStock` |
| `ProductInStockUpdateCommnand` | Typo in the class name | `ProductInStockUpdateCommand` |
| `DefaultController` | Returns a `string` to show the service is alive | `services.AddHealthChecks()` + `endpoints.MapHealthChecks("/health")` |
| Controllers | No `CancellationToken` | Add a `CancellationToken ct` parameter and pass it down to EF/MediatR |
| No validation attributes on the commands | Empty name or price ≤ 0 is accepted | DataAnnotations or FluentValidation → automatic 400 |
| No global error handling | Exceptions → developer page or a bare 500 | `ExceptionMiddleware` + `ProblemDetails` |

---

## 13. Quick interview Q&A

- **Why `ControllerBase` and not `Controller`?** `Controller` adds view support (`View()`, `TempData`) that APIs don't need.
- **What does `[ApiController]` give you?** Attribute routing is required, invalid ModelState becomes an automatic 400, binding sources are inferred (`[FromBody]` for complex types), and errors come back as ProblemDetails.
- **`IActionResult` vs `ActionResult<T>`?** `ActionResult<T>` lets you return either `T` or any result, and Swagger can infer the schema.
- **POST vs PUT?** POST: the server picks the URI, and it's not idempotent. PUT: the client knows the URI, it replaces the resource, and it's idempotent.
- **What goes in a 201?** A `Location` header pointing at the new resource, usually with the created body.
- **400 vs 422?** 400: malformed or invalid input. 422: well-formed, but it breaks a business rule. Many teams use 400 for both; just be consistent.
- **409 vs 412?** 409: the request conflicts with the current state (duplicate key, wrong state). 412: an `If-Match`/ETag precondition failed.
- **Should a missing item in a list endpoint be 404?** No. Return `200` with an empty list. 404 is for a specific resource URI.
- **Where do business rules go?** In the command handler or domain, not the controller. Controllers stay thin: bind → send → map the result to a status code.
- **Why async + CancellationToken?** It frees threads while waiting on I/O, and it lets you stop the work when the client disconnects.
- **Middleware order?** ExceptionHandler → HSTS/HttpsRedirection → Routing → CORS → Authentication → Authorization → Endpoints.
- **DI lifetimes?** `Transient`: a new instance every time. `Scoped`: one per request (DbContext). `Singleton`: one for the app. Never inject a Scoped service into a Singleton.
- **Return a DTO or the entity?** Always a DTO. Returning the entity leaks the schema, creates circular references, and risks over-posting.

---

## 14. Copy-paste skeleton

```csharp
[ApiController]
[Route("api/[controller]")]
public class ThingsController : ControllerBase
{
    [HttpGet]                    public ActionResult<IEnumerable<ThingDto>> GetAll()          => Ok(...);
    [HttpGet("{id:int}")]        public ActionResult<ThingDto> Get(int id)                     => found ? dto : NotFound();
    [HttpPost]                   public ActionResult<ThingDto> Create(CreateThing c)           => CreatedAtAction(nameof(Get), new { id }, dto);
    [HttpPut("{id:int}")]        public IActionResult Update(int id, UpdateThing c)           => found ? NoContent() : NotFound();
    [HttpPatch("{id:int}")]      public IActionResult Patch(int id, JsonPatchDocument<ThingDto> p) => NoContent();
    [HttpDelete("{id:int}")]     public IActionResult Delete(int id)                           => found ? NoContent() : NotFound();
}
```
