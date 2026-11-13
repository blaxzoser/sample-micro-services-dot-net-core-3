# Mock Interview: Practical ASP.NET Core Web API Exercises (Senior)

Timed exercises in the style of Codility and HackerRank. **Each exercise is one small scenario**, not a full CRUD. Set a timer, code it, run the test cases, and only then open the solution.

> Companion reference: [aspnet-controller-cheatsheet.md](aspnet-controller-cheatsheet.md)

| Level | Time box | What it checks |
|---|---|---|
| 🟢 Basic | **15 min** | Correct verb, route, status code, and ControllerBase helper |
| 🟡 Intermediate | **20 min** | Validation, ProblemDetails, business-rule codes (409/422/402) |
| 🔴 Advanced | **25–30 min** | Headers (ETag, If-Match, Idempotency-Key), concurrency, filters |

**Rules**
1. Start the timer before you read the scenario.
2. Don't open the solution until the timer ends or every test case passes.
3. Talk out loud, as you would in an interview: explain *why* you chose each status code.
4. Restart the app between exercises. The in-memory data resets on restart.

---

## Progress tracker

| # | Exercise | Level | Time | Status codes | Done |
|---|---|---|---|---|---|
| POST-1 | Submit feedback | 🟢 | 15 | 200, 400 | ☐ |
| POST-2 | Create product | 🟢 | 15 | 201, 400 | ☐ |
| POST-3 | Reject duplicate SKU | 🟡 | 20 | 201, 409 | ☐ |
| POST-4 | Pay an order | 🟡 | 20 | 200, 402, 404, 409 | ☐ |
| POST-5 | Idempotent payment | 🔴 | 30 | 201, 400, 422 | ☐ |
| POST-6 | Async report job | 🔴 | 25 | 202, 200, 404 | ☐ |
| GET-1 | Product by id | 🟢 | 15 | 200, 404 | ☐ |
| GET-2 | Filter by category | 🟢 | 15 | 200 (empty list) | ☐ |
| GET-3 | Pagination | 🟡 | 20 | 200, 400 + header | ☐ |
| GET-4 | Nested resource | 🟡 | 20 | 200, 400, 404 | ☐ |
| GET-5 | Conditional GET (ETag) | 🔴 | 25 | 200, 304 | ☐ |
| GET-6 | Customers over 30 + payment count | 🟡 | 20 | 200 (empty list) | ☐ |
| GET-7 | Order summary by age range | 🟡 | 25 | 200, 400 | ☐ |
| GET-8 | Customer activity (orders + payments) | 🟢 | 15 | 200 (empty list) | ☐ |
| PUT-1 | Replace product | 🟢 | 15 | 204, 400, 404 | ☐ |
| PUT-2 | Replace user (id mismatch) | 🟢 | 15 | 200, 400, 404 | ☐ |
| PUT-3 | Upsert setting | 🟡 | 20 | 201, 204, 400 | ☐ |
| PUT-4 | Price rule (400 vs 422) | 🟡 | 20 | 204, 400, 422 | ☐ |
| PUT-5 | Optimistic concurrency | 🔴 | 30 | 204, 412, 428 | ☐ |
| PATCH-1 | Partial user update | 🟢 | 15 | 200, 400, 404 | ☐ |
| PATCH-2 | Change order status | 🟢 | 15 | 200, 400, 404 | ☐ |
| PATCH-3 | Status state machine | 🟡 | 20 | 200, 409 | ☐ |
| PATCH-4 | JSON Patch document | 🟡 | 25 | 200, 400, 404, 415 | ☐ |
| PATCH-5 | Concurrent stock adjust | 🔴 | 30 | 200, 400, 409 | ☐ |
| DELETE-1 | Delete product | 🟢 | 15 | 204, 404 | ☐ |
| DELETE-2 | Idempotent cart item delete | 🟢 | 15 | 204, 404 | ☐ |
| DELETE-3 | Block delete with dependents | 🟡 | 20 | 204, 409 | ☐ |
| DELETE-4 | Soft delete + 410 Gone | 🟡 | 20 | 204, 410 | ☐ |
| DELETE-5 | API-key protected delete | 🔴 | 30 | 204, 401, 403 | ☐ |

**Suggested plan:** day 1 all 🟢, day 2 all 🟡, day 3 all 🔴, day 4 redo the ones that went over time.

---

## 0. Starter kit (set up once, ~5 min)

Online platforms usually hand you a project like this. Build it once and reuse it for every exercise.

```bash
dotnet new webapi -n MockApi --use-controllers
```
Delete `WeatherForecast.cs` and `Controllers/WeatherForecastController.cs`.

> Target: **.NET 8 or later** (the solutions use `[ProducesResponseType<T>]` and typed header properties such as `Response.Headers.ETag`). This repo runs .NET Core 3.1. On 3.1, write `[ProducesResponseType(typeof(T), 200)]` and `Response.Headers["ETag"]` instead.

**GlobalUsings.cs**, so every snippet compiles without extra `using` lines:
```csharp
global using System.Collections.Concurrent;
global using System.ComponentModel.DataAnnotations;
global using System.Text.RegularExpressions;
global using Microsoft.AspNetCore.Mvc;
```

**Program.cs**
```csharp
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<Db>();          // in-memory "database", resets on restart

var app = builder.Build();
app.MapControllers();
app.Run();

public partial class Program { }              // lets tests use WebApplicationFactory<Program>
```

**Models.cs**
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; }
    public int Version { get; set; } = 1;     // used for ETags
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Balance { get; set; }
    public DateTime Birthday { get; set; }     // GET-6
}

public enum OrderStatus { Pending, Paid, Shipped, Delivered, Cancelled }

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Payment
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**Db.cs**
```csharp
public class Db
{
    public readonly object Sync = new();      // lock this for check-then-write operations

    public List<Product> Products { get; } = new()
    {
        new() { Id = 1, Name = "Laptop", Sku = "LAP-001", Category = "Electronics", Price = 1200m, Stock = 10 },
        new() { Id = 2, Name = "Mouse",  Sku = "MOU-001", Category = "Electronics", Price = 25m,   Stock = 100 },
        new() { Id = 3, Name = "Desk",   Sku = "DSK-001", Category = "Furniture",   Price = 300m,  Stock = 5 },
    };

    public List<Customer> Customers { get; } = new()
    {
        new() { Id = 1, Name = "Ana",  Balance = 500m, Birthday = new DateTime(1987, 1, 13) },
        new() { Id = 2, Name = "Luis", Balance = 20m,  Birthday = new DateTime(2000, 6, 15) },
        new() { Id = 3, Name = "Eva",  Balance = 0m,   Birthday = new DateTime(1980, 3, 2) },  // has no orders
    };

    public List<Order> Orders { get; } = new()
    {
        new() { Id = 1, CustomerId = 1, Total = 100m, Status = OrderStatus.Pending },
        new() { Id = 2, CustomerId = 2, Total = 100m, Status = OrderStatus.Pending },
        new() { Id = 3, CustomerId = 1, Total = 50m,  Status = OrderStatus.Paid },
        new() { Id = 4, CustomerId = 1, Total = 70m,  Status = OrderStatus.Delivered },
    };

    public List<User> Users { get; } = new()
    {
        new() { Id = 1, Name = "Ana Torres", Email = "ana@mail.com" },
        new() { Id = 2, Name = "Luis Perez", Email = "luis@mail.com", Phone = "+51 999 111 222" },
    };

    public List<Payment> Payments { get; } = new();

    public Dictionary<string, string> Settings { get; } = new() { ["theme"] = "dark" };

    public Dictionary<int, HashSet<int>> Carts { get; } = new() { [1] = new() { 1, 2 } }; // cartId -> productIds

    public int NextId<T>(List<T> list, Func<T, int> id) => list.Count == 0 ? 1 : list.Max(id) + 1;
}
```

**How to test:** use the `.http` file that Visual Studio or Rider creates, Swagger, or curl. Check `Properties/launchSettings.json` for the port.
```http
@host = http://localhost:5000

POST {{host}}/api/feedback
Content-Type: application/json

{ "message": "Great service", "rating": 5 }
```

> **Hidden tests** on these platforms usually check: **status code**, **response body shape** (property names in camelCase), **headers** (`Location`, `ETag`), and **state** (a follow-up GET). Every row in a test case table assumes **fresh seed data**.

---

# POST

## POST-1 · Submit feedback → 200 / 400 🟢 15 min

**Scenario:** A support page sends customer feedback. Return the stored feedback with a generated id.

**Endpoint:** `POST /api/feedback`

**Requirements**
- Body: `{ "message": string, "rating": int }`
- `message` is required: not empty, not only spaces, max 500 characters.
- `rating` must be between 1 and 5.
- Success → **200 OK** with `{ id, message, rating, receivedAt }`, ids starting at 1.
- Invalid input → **400** with a ProblemDetails body that lists the field errors.

**Test cases**
| Request body | Expected |
|---|---|
| `{"message":"Great","rating":5}` | 200, `id: 1` |
| `{"message":"Good","rating":4}` (second call) | 200, `id: 2` |
| `{"message":"","rating":3}` | 400, `errors.Message` present |
| `{"message":"   ","rating":3}` | 400 |
| `{"message":"Ok","rating":9}` | 400, `errors.Rating` present |
| `{"message":"Ok"}` | 400 (rating defaults to 0) |
| body `not json` | 400 |
| `Content-Type: text/plain` | 415 |

**Interviewer follow-ups:** Why 200 and not 201 here? (There is no addressable resource, meaning no GET URL.) Who produces the 400: you or the framework? Is a `static int` counter thread-safe?

<details>
<summary>✅ Solution</summary>

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class FeedbackRequest
{
    [Required, StringLength(500)]          // Required rejects "" and whitespace
    public string Message { get; set; } = "";

    [Range(1, 5)]
    public int Rating { get; set; }
}

public record FeedbackResponse(int Id, string Message, int Rating, DateTime ReceivedAt);

[ApiController]                             // automatic 400 ValidationProblemDetails
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private static int _lastId;

    [HttpPost]
    [ProducesResponseType<FeedbackResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<FeedbackResponse> Create(FeedbackRequest request)
    {
        var id = Interlocked.Increment(ref _lastId);   // thread-safe counter
        return Ok(new FeedbackResponse(id, request.Message.Trim(), request.Rating, DateTime.UtcNow));
    }
}
```
Key point: you write **no `if` statements for validation**. `[ApiController]` and DataAnnotations return the 400 before the action runs.
</details>

---

## POST-2 · Create product → 201 Created / 400 🟢 15 min

**Scenario:** Admins create products. The client needs the URL of the new product.

**Endpoint:** `POST /api/products`

**Requirements**
- Body: `{ name, sku, category, price, stock }`
- `name` is required, max 100 characters. `sku` is required and matches `AAA-000` (3 uppercase letters, a dash, 3 digits). `category` is required. `price` is from 0.01 to 1,000,000. `stock` is 0 or more.
- Success → **201 Created**, a `Location: /api/products/{id}` header, and the created product in the body.
- `GET /api/products/{id}` must work so the Location URL resolves.
- Invalid input → **400**.

**Test cases**
| Request | Expected |
|---|---|
| `{"name":"Chair","sku":"CHR-001","category":"Furniture","price":80,"stock":3}` | 201, `id: 4`, `Location` ends with `/api/products/4` |
| GET the Location URL | 200, `name: "Chair"` |
| `sku: "chair1"` | 400, `errors.Sku` |
| `price: 0` | 400, `errors.Price` |
| `stock: -1` | 400 |

**Follow-ups:** `CreatedAtAction` vs `CreatedAtRoute` vs `Created`? What happens if your GET action is named `GetByIdAsync`? (Answer: `SuppressAsyncSuffixInActionNames` is true by default, so the route lookup for `nameof(GetByIdAsync)` fails at runtime. Use `[ActionName]` or turn that option off.)

<details>
<summary>✅ Solution</summary>

```csharp
public class CreateProductRequest
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, RegularExpression(@"^[A-Z]{3}-\d{3}$")] public string Sku { get; set; } = "";
    [Required] public string Category { get; set; } = "";
    [Range(typeof(decimal), "0.01", "1000000")] public decimal Price { get; set; }
    [Range(0, int.MaxValue)] public int Stock { get; set; }
}

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly Db _db;
    public ProductsController(Db db) => _db = db;

    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        return product is null ? NotFound() : product;
    }

    [HttpPost]
    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> Create(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name, Sku = request.Sku, Category = request.Category,
            Price = request.Price, Stock = request.Stock
        };

        lock (_db.Sync)
        {
            product.Id = _db.NextId(_db.Products, p => p.Id);
            _db.Products.Add(product);
        }

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
```
</details>

---

## POST-3 · Reject duplicate SKU → 409 Conflict 🟡 20 min

**Scenario:** The SKU must be unique. A second product with the same SKU conflicts with the current state.

**Endpoint:** `POST /api/products` (extends POST-2)

**Requirements**
- Duplicate check is **case-insensitive**: `lap-001` conflicts with `LAP-001`. For this exercise, relax the regex to `^[A-Za-z]{3}-\d{3}$`.
- Duplicate → **409** ProblemDetails: `title: "Duplicate SKU"`, with `detail` naming the SKU.
- Two parallel requests with the same new SKU → exactly one **201** and one **409**.

**Test cases**
| Request | Expected |
|---|---|
| `sku: "LAP-001"` | 409, `title: "Duplicate SKU"` |
| `sku: "lap-001"` | 409 |
| `sku: "NEW-001"` | 201 |
| `sku: "NEW-001"` again | 409 |

**Follow-ups:** 409 or 400 or 422 for duplicates? How do you enforce this in SQL Server? (A unique index, then catch `DbUpdateException` with error 2601/2627 and map it to 409. Checking first and inserting after is a race condition.)

<details>
<summary>✅ Solution</summary>

```csharp
[HttpPost]
[ProducesResponseType<Product>(StatusCodes.Status201Created)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
public ActionResult<Product> Create(CreateProductRequest request)
{
    lock (_db.Sync)   // the check and the insert must be atomic
    {
        var exists = _db.Products.Any(p => p.Sku.Equals(request.Sku, StringComparison.OrdinalIgnoreCase));
        if (exists)
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Duplicate SKU",
                detail: $"A product with SKU '{request.Sku}' already exists.");

        var product = new Product
        {
            Id = _db.NextId(_db.Products, p => p.Id),
            Name = request.Name, Sku = request.Sku.ToUpperInvariant(),
            Category = request.Category, Price = request.Price, Stock = request.Stock
        };
        _db.Products.Add(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
```
</details>

---

## POST-4 · Pay an order → 200 / 402 Payment Required / 404 / 409 🟡 20 min

**Scenario:** A customer pays for an order from their wallet balance.

**Endpoint:** `POST /api/orders/{id}/pay` (an action endpoint with no body)

**Requirements**
- Order not found → **404**.
- Order is not `Pending` → **409** (`title: "Order is not payable"`).
- Customer balance < order total → **402 Payment Required** (`title: "Insufficient funds"`), with `balance` and `required` in the ProblemDetails extensions.
- Success → subtract the total from the balance, set the status to `Paid`, and return **200** with `{ orderId, status, remainingBalance }`.
- **Check in this order:** 404 → 409 → 402.

**Test cases** (seed: Ana has 500, Luis has 20)
| Request | Expected |
|---|---|
| `POST /api/orders/1/pay` | 200, `status: "Paid"`, `remainingBalance: 400` |
| `POST /api/orders/1/pay` again | 409 |
| `POST /api/orders/2/pay` | 402, `required: 100`, `balance: 20` |
| `POST /api/orders/3/pay` | 409 (already Paid) |
| `POST /api/orders/999/pay` | 404 |
| `POST /api/orders/abc/pay` | 404 (route constraint) |

**Follow-ups:** 402 is "reserved for future use" in the spec. Would you use it in production, or use 422 or 409? Why POST and not PATCH for this? (It's a command with side effects, not an edit to a field.)

<details>
<summary>✅ Solution</summary>

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly Db _db;
    public OrdersController(Db db) => _db = db;

    public record PayResponse(int OrderId, OrderStatus Status, decimal RemainingBalance);

    [HttpPost("{id:int}/pay")]
    [ProducesResponseType<PayResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public ActionResult<PayResponse> Pay(int id)
    {
        lock (_db.Sync)
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == id);
            if (order is null) return NotFound();

            if (order.Status != OrderStatus.Pending)
                return Problem(statusCode: StatusCodes.Status409Conflict,
                               title: "Order is not payable",
                               detail: $"Order {id} is {order.Status}.");

            var customer = _db.Customers.Single(c => c.Id == order.CustomerId);
            if (customer.Balance < order.Total)
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status402PaymentRequired,
                    Title = "Insufficient funds",
                    Detail = $"Order {id} requires {order.Total}."
                };
                problem.Extensions["balance"] = customer.Balance;
                problem.Extensions["required"] = order.Total;
                return StatusCode(StatusCodes.Status402PaymentRequired, problem);
            }

            customer.Balance -= order.Total;
            order.Status = OrderStatus.Paid;
            return Ok(new PayResponse(order.Id, order.Status, customer.Balance));
        }
    }
}
```
</details>

---

## POST-5 · Idempotent payment (Idempotency-Key) 🔴 30 min

**Scenario:** Mobile clients retry on timeouts. A retry must **never charge twice**.

**Endpoints:** `POST /api/payments`, `GET /api/payments/{id}`

**Requirements**
- Header `Idempotency-Key` is required. If it's missing or blank → **400**.
- Body: `{ customerId, amount }`. `amount` is from 0.01 to 100,000. Unknown customer → **404**.
- First request with a key → create the payment → **201** + `Location`.
- Same key + same body → return the **same** payment (same id, still 201), add the header `Idempotent-Replayed: true`, and **don't** create a new record.
- Same key + **different** body → **422** (`title: "Idempotency-Key reused with a different payload"`).

**Test cases**
| Request | Expected |
|---|---|
| key `k1`, `{customerId:1, amount:10}` | 201, `id: 1` |
| key `k1`, same body | 201, `id: 1`, `Idempotent-Replayed: true` |
| `GET /api/payments` count (debug) | still 1 payment |
| key `k1`, `{customerId:1, amount:99}` | 422 |
| no key header | 400 |
| key `k2`, `{customerId:999, amount:5}` | 404 |

**Follow-ups:** Where would you store keys in a cluster with 3 instances? (Redis or the database, with a TTL.) What if two identical requests arrive at the same time? (A lock, or a unique constraint on the key.) Why should an action filter or middleware handle this instead of each controller?

<details>
<summary>✅ Solution</summary>

```csharp
public class CreatePaymentRequest
{
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    [Range(typeof(decimal), "0.01", "100000")] public decimal Amount { get; set; }
}

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly Db _db;
    private static readonly Dictionary<string, (CreatePaymentRequest Request, Payment Result)> Processed = new();

    public PaymentsController(Db db) => _db = db;

    [HttpGet("{id:int}")]
    public ActionResult<Payment> GetById(int id)
        => _db.Payments.FirstOrDefault(p => p.Id == id) is { } p ? p : NotFound();

    [HttpPost]
    public ActionResult<Payment> Create(
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,   // nullable so we write our own 400
        CreatePaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                           title: "Idempotency-Key header is required");

        lock (_db.Sync)
        {
            if (Processed.TryGetValue(idempotencyKey, out var previous))
            {
                var samePayload = previous.Request.CustomerId == request.CustomerId
                               && previous.Request.Amount == request.Amount;
                if (!samePayload)
                    return Problem(statusCode: StatusCodes.Status422UnprocessableEntity,
                                   title: "Idempotency-Key reused with a different payload");

                Response.Headers["Idempotent-Replayed"] = "true";
                return CreatedAtAction(nameof(GetById), new { id = previous.Result.Id }, previous.Result);
            }

            if (!_db.Customers.Any(c => c.Id == request.CustomerId))
                return Problem(statusCode: StatusCodes.Status404NotFound,
                               title: "Customer not found");

            var payment = new Payment
            {
                Id = _db.NextId(_db.Payments, p => p.Id),
                CustomerId = request.CustomerId,
                Amount = request.Amount
            };
            _db.Payments.Add(payment);
            Processed[idempotencyKey] = (request, payment);

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }
    }
}
```
Gotcha: with nullable reference types on, a **non-nullable** `string idempotencyKey` is implicitly `[Required]`, so `[ApiController]` returns its own 400 before your code runs. Use `string?` if you want to control that response.
</details>

---

## POST-6 · Long-running report → 202 Accepted 🔴 25 min

**Scenario:** Generating a report takes time. Don't block the request. Accept it and let the client poll.

**Endpoints:** `POST /api/reports`, `GET /api/reports/{id}`

**Requirements**
- Body: `{ "type": "sales" | "stock" }`. Any other value → **400**.
- POST → **202 Accepted** + `Location: /api/reports/{guid}` + body `{ jobId, status: "Pending" }`.
- GET within 5 seconds of creation → **200** `{ status: "Pending" }` + header `Retry-After: 5`.
- GET after 5 seconds → **200** `{ status: "Completed", downloadUrl }`. Simulate the work by comparing timestamps; no background thread is needed.
- Unknown id → **404**. A non-GUID id → **404** (route constraint).

**Test cases**
| Request | Expected |
|---|---|
| `POST {"type":"sales"}` | 202, `Location` present |
| GET Location immediately | 200, `Pending`, `Retry-After: 5` |
| GET after 6s | 200, `Completed` |
| `POST {"type":"invoices"}` | 400 |
| `GET /api/reports/{random guid}` | 404 |

**Follow-ups:** 202 vs 201? Real implementation? (A queue such as Azure Service Bus or RabbitMQ, a `BackgroundService` or Hangfire worker, and job state in a database.) When would you return 303 See Other?

<details>
<summary>✅ Solution</summary>

```csharp
public class CreateReportRequest
{
    [Required, RegularExpression("^(sales|stock)$")]
    public string Type { get; set; } = "";
}

public record ReportJob(Guid Id, string Type, DateTime CreatedAt);

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private static readonly ConcurrentDictionary<Guid, ReportJob> Jobs = new();
    private static readonly TimeSpan ProcessingTime = TimeSpan.FromSeconds(5);

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Create(CreateReportRequest request)
    {
        var job = new ReportJob(Guid.NewGuid(), request.Type, DateTime.UtcNow);
        Jobs[job.Id] = job;
        return AcceptedAtAction(nameof(GetStatus), new { id = job.Id }, new { jobId = job.Id, status = "Pending" });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetStatus(Guid id)
    {
        if (!Jobs.TryGetValue(id, out var job)) return NotFound();

        if (DateTime.UtcNow - job.CreatedAt < ProcessingTime)
        {
            Response.Headers.RetryAfter = "5";
            return Ok(new { jobId = id, status = "Pending" });
        }

        return Ok(new { jobId = id, status = "Completed", downloadUrl = $"/api/reports/{id}/file" });
    }
}
```
</details>

---

# GET

## GET-1 · Product by id → 200 / 404 🟢 15 min

**Endpoint:** `GET /api/products/{id}`

**Requirements**
- Found → **200** with the product.
- Not found, or `IsDeleted == true` → **404**.
- Non-numeric id → **404** (use a route constraint).
- Document both responses with `[ProducesResponseType]`.

**Test cases**
| Request | Expected |
|---|---|
| `GET /api/products/1` | 200, `name: "Laptop"` |
| `GET /api/products/999` | 404 |
| `GET /api/products/abc` | 404 |
| `GET /api/products/0` | 404 |

**Follow-ups:** Without `:int`, what does `/abc` return? (400 from model binding with `[ApiController]`.) Why not 204 for not found? Why `ActionResult<T>` instead of `Task<Product>`? (The concrete type can't express 404. Returning `null` gives a 204.)

<details>
<summary>✅ Solution</summary>

```csharp
[HttpGet("{id:int}")]
[ProducesResponseType<Product>(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public ActionResult<Product> GetById(int id)
{
    var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
    if (product is null) return NotFound();
    return product;          // implicit 200 via ActionResult<T>
}
```
With EF Core, use `await _db.Products.FirstOrDefaultAsync(..., ct)`, **not** `SingleAsync`, which throws and gives a 500. That's the bug in this repo's `ProductQueryService.GetAsyc`.
</details>

---

## GET-2 · Filter by category → 200 (with an empty list) 🟢 15 min

**Endpoint:** `GET /api/products?category={name}`

**Requirements**
- No `category` → all products that aren't deleted.
- Matching is **case-insensitive**.
- Unknown category → **200 with `[]`**, not a 404.
- Return a DTO `{ id, name, category, price }`. Don't expose `sku`, `stock`, `version`, or `isDeleted`.

**Test cases**
| Request | Expected |
|---|---|
| `GET /api/products` | 200, 3 items |
| `?category=electronics` | 200, 2 items |
| `?category=FURNITURE` | 200, 1 item |
| `?category=toys` | 200, `[]` |
| check any item | has no `sku` or `stock` properties |

**Follow-ups:** Why is 404 wrong for an empty search? Why use DTOs? (Over-posting and over-exposure, and the contract stays stable when entities change.) Where would you use `AsNoTracking()`?

<details>
<summary>✅ Solution</summary>

```csharp
public record ProductDto(int Id, string Name, string Category, decimal Price);

[HttpGet]
[ProducesResponseType<IEnumerable<ProductDto>>(StatusCodes.Status200OK)]
public ActionResult<IEnumerable<ProductDto>> GetAll([FromQuery] string? category)
{
    var query = _db.Products.Where(p => !p.IsDeleted);

    if (!string.IsNullOrWhiteSpace(category))
        query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

    return Ok(query.Select(p => new ProductDto(p.Id, p.Name, p.Category, p.Price)).ToList());
}
```
</details>

---

## GET-3 · Pagination + X-Total-Count → 200 / 400 🟡 20 min

**Endpoint:** `GET /api/products/paged?page=1&pageSize=2`

**Requirements**
- Defaults: `page=1`, `pageSize=10`. `page` must be 1 or more, and `pageSize` must be from 1 to 50 → otherwise **400**.
- Order by `id`.
- Body: `{ items, page, pageSize, totalCount, totalPages }`.
- Header `X-Total-Count: {totalCount}`.
- A page past the end → **200** with empty `items`.

**Test cases**
| Request | Expected |
|---|---|
| `?page=1&pageSize=2` | 200, 2 items (ids 1, 2), `totalCount: 3`, `totalPages: 2`, header `X-Total-Count: 3` |
| `?page=2&pageSize=2` | 200, 1 item (id 3) |
| `?page=5&pageSize=2` | 200, `items: []` |
| `?page=0` | 400 |
| `?pageSize=100` | 400 |
| `?page=abc` | 400 |

**Follow-ups:** Offset paging vs cursor (keyset) paging? Why is a stable `OrderBy` required before `Skip`/`Take`? How does this repo's `PagingExtension.GetPagedAsync` compare?

<details>
<summary>✅ Solution</summary>

```csharp
public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);

[HttpGet("paged")]
[ProducesResponseType<PagedResult<ProductDto>>(StatusCodes.Status200OK)]
[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
public ActionResult<PagedResult<ProductDto>> GetPaged(
    [FromQuery, Range(1, int.MaxValue)] int page = 1,      // [ApiController] validates parameters too
    [FromQuery, Range(1, 50)] int pageSize = 10)
{
    var query = _db.Products.Where(p => !p.IsDeleted).OrderBy(p => p.Id);
    var total = query.Count();

    var items = query.Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .Select(p => new ProductDto(p.Id, p.Name, p.Category, p.Price))
                     .ToList();

    Response.Headers["X-Total-Count"] = total.ToString();
    var totalPages = (int)Math.Ceiling(total / (double)pageSize);

    return Ok(new PagedResult<ProductDto>(items, page, pageSize, total, totalPages));
}
```
</details>

---

## GET-4 · Nested resource: a customer's orders → 200 / 400 / 404 🟡 20 min

**Endpoint:** `GET /api/customers/{customerId}/orders?status=Pending`

**Requirements**
- Customer doesn't exist → **404** ProblemDetails (`title: "Customer not found"`).
- Customer exists with no orders → **200 `[]`**.
- Optional `status` filter (enum name). Invalid value → **400**.

**Test cases**
| Request | Expected |
|---|---|
| `/api/customers/1/orders` | 200, 3 orders |
| `/api/customers/1/orders?status=Pending` | 200, 1 order (id 1) |
| `/api/customers/3/orders` | 200, `[]` |
| `/api/customers/999/orders` | 404 |
| `/api/customers/1/orders?status=Flying` | 400 |

**Follow-ups:** Why is a 404 for the parent different from an empty list for the children? Nested routes vs `/api/orders?customerId=1`: when would you choose each?

<details>
<summary>✅ Solution</summary>

```csharp
[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly Db _db;
    public CustomersController(Db db) => _db = db;

    [HttpGet("{customerId:int}/orders")]
    [ProducesResponseType<IEnumerable<Order>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<Order>> GetOrders(int customerId, [FromQuery] OrderStatus? status)
    {
        if (!_db.Customers.Any(c => c.Id == customerId))
            return Problem(statusCode: StatusCodes.Status404NotFound,
                           title: "Customer not found",
                           detail: $"Customer {customerId} does not exist.");

        var orders = _db.Orders
            .Where(o => o.CustomerId == customerId && (status == null || o.Status == status))
            .ToList();

        return Ok(orders);
    }
}
```
</details>

---

## GET-5 · Conditional GET with ETag → 200 / 304 🔴 25 min

**Scenario:** Mobile clients poll a product often. Save bandwidth: if nothing changed, return no body.

**Endpoint:** `GET /api/products/{id}` (extends GET-1)

**Requirements**
- Every 200 response includes `ETag: "{Version}"`, for example `"1"`, and `Cache-Control: private, max-age=60`.
- If the request's `If-None-Match` equals the current ETag, or is `*` → **304 Not Modified** with no body, still sending the `ETag` header.
- If it doesn't match → **200** with the body and the new ETag.
- Not found → **404**.

**Test cases**
| Request | Expected |
|---|---|
| `GET /api/products/1` | 200, `ETag: "1"` |
| same + `If-None-Match: "1"` | 304, empty body, `ETag: "1"` |
| same + `If-None-Match: "0"` | 200 |
| same + `If-None-Match: *` | 304 |
| `GET /api/products/999` + `If-None-Match: "1"` | 404 |

**Follow-ups:** Strong vs weak ETags (`W/"1"`)? Version number vs content hash? Could this be middleware or an action filter? What does `ResponseCaching` do differently?

<details>
<summary>✅ Solution</summary>

```csharp
[HttpGet("{id:int}")]
[ProducesResponseType<Product>(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status304NotModified)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult GetById(int id)
{
    var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
    if (product is null) return NotFound();

    var etag = $"\"{product.Version}\"";
    Response.Headers.ETag = etag;
    Response.Headers.CacheControl = "private, max-age=60";

    var ifNoneMatch = Request.Headers.IfNoneMatch.ToString();
    if (ifNoneMatch == etag || ifNoneMatch == "*")
        return StatusCode(StatusCodes.Status304NotModified);

    return Ok(product);
}
```
Production version: parse `Request.GetTypedHeaders().IfNoneMatch`, which handles lists like `"1", "2"` and weak tags.
</details>

---

## GET-6 · Customers over 30 with their payment count → 200 🟡 20 min

**Scenario:** Marketing wants a list of customers older than 30 and how many payments each has made.

**Endpoint:** `GET /api/customers/over-30`

**Setup (this exercise only):** the starter kit has no payments. Seed these in `Db.Payments` for this exercise. POST-5's test cases assume the list starts empty, so take them out again before you do POST-5.
```csharp
public List<Payment> Payments { get; } = new()
{
    new() { Id = 1, CustomerId = 1, Amount = 100m },
    new() { Id = 2, CustomerId = 1, Amount = 50m },
    new() { Id = 3, CustomerId = 1, Amount = 70m },
    new() { Id = 4, CustomerId = 2, Amount = 100m },
};
```

**Requirements**
- Only customers whose **age** is more than 30, so 31 or older. Work out age from `Birthday` and today's date in whole years: someone who is 30 years and 11 months old is 30.
- Body: a list of `{ customerName, age, totalPayments }`, ordered by `customerName`. Use a DTO; don't return the entity.
- `totalPayments` is the **number** of payments. A customer with no payments still appears, with `0`.
- No customer matches → **200 `[]`**.

**Test cases** (ages as of 2026-11-12; they go up on each birthday)
| Request | Expected |
|---|---|
| `GET /api/customers/over-30` | 200, 2 items |
| item 1 | `{ customerName: "Ana", age: 39, totalPayments: 3 }` |
| item 2 | `{ customerName: "Eva", age: 46, totalPayments: 0 }` |
| check the list | no `Luis` (age 26) |
| check any item | has no `id`, `balance` or `birthday` properties |

**Follow-ups:** Why is `today.Year - birthday.Year` alone wrong? Why filter with `Birthday <= today.AddYears(-31)` instead of computing age in the `Where`? (A plain comparison against a constant can use an index; a calculation on the column can't.) Inner join + group by vs a correlated `Count`: which one drops Eva? How would you make the test cases stop depending on today's date? (Inject `TimeProvider` and use `FakeTimeProvider` in tests.)

<details>
<summary>✅ Solution</summary>

```csharp
public record CustomerPaymentsSummaryDto(string CustomerName, int Age, int TotalPayments);

[HttpGet("over-30")]
[ProducesResponseType<IEnumerable<CustomerPaymentsSummaryDto>>(StatusCodes.Status200OK)]
public ActionResult<IEnumerable<CustomerPaymentsSummaryDto>> GetOver30()
{
    var today = DateTime.Today;
    var cutoff = today.AddYears(-31);          // born on or before this date => 31 or older

    var result = _db.Customers
        .Where(c => c.Birthday <= cutoff)
        .OrderBy(c => c.Name)
        .Select(c => new CustomerPaymentsSummaryDto(
            c.Name,
            AgeOn(c.Birthday, today),
            _db.Payments.Count(p => p.CustomerId == c.Id)))   // 0 when there are none
        .ToList();

    return Ok(result);
}

private static int AgeOn(DateTime birthday, DateTime today)
{
    var age = today.Year - birthday.Year;
    if (birthday.Date > today.AddYears(-age)) age--;     // birthday hasn't happened yet this year
    return age;
}
```
**With EF Core** (`src/Services/Customer/Customer.Api`): EF can't translate `AgeOn` to SQL. In the final `Select`, EF runs it in C# after the query, so the code above still works with `await ...ToListAsync(ct)`. The same call inside a `Where` would throw, which is one more reason to filter on `cutoff`. The project's version is easier to follow: it selects `Name`, `Birthday` and the count, then calculates the age in a second step. EF turns the `Count` into a correlated `SELECT COUNT(*)` subquery, so Eva still appears.
</details>

---

## GET-7 · Order summary for an age range → 200 / 400 🟡 25 min

**Scenario:** Sales wants to compare customer groups by age: how many orders each customer placed and how much money was actually collected from them.

**Endpoint:** `GET /api/customers/order-summary?minAge=25&maxAge=40`

**Same skills as GET-6:** start from the parent (customers), keep the ones with no children (zeros), filter on dates instead of calculating age in SQL, and calculate age in C#. **New in GET-7:** a range filter, query-string validation, two aggregates (a count and a conditional sum), and sorting on a calculated value.

Uses the starter-kit seed as it is: Ana has 3 orders (Pending 100, Paid 50, Delivered 70), Luis has 1 (Pending 100), and Eva has none.

**Requirements**
- `minAge` and `maxAge` are optional query parameters, defaulting to `18` and `120`. Each must be from 0 to 150, and `minAge` can't be greater than `maxAge`. Otherwise → **400** ValidationProblemDetails.
- Age range is **inclusive**: `minAge=39&maxAge=39` includes someone who is 39.
- Body: a list of `{ customerName, age, totalOrders, totalSpent }`.
  - `totalOrders` = number of orders, **any** status.
  - `totalSpent` = sum of `Total` for orders that are **`Paid` or `Delivered`** (money collected). `Pending` and `Cancelled` don't count.
  - A customer with no orders appears with `0` and `0`.
- Order by `totalSpent` **descending**, then `customerName` ascending, so ties come out in a stable order.
- No customer in range → **200 `[]`**.

**Test cases** (ages as of 2026-11-12: Ana 39, Luis 26, Eva 46; valid until Ana's birthday on 2027-01-13)
| Request | Expected |
|---|---|
| `?minAge=25&maxAge=40` | 200: Ana `{age: 39, totalOrders: 3, totalSpent: 120}`, then Luis `{26, 1, 0}` |
| no parameters | 200: Ana (120), Eva (0), Luis (0). Eva comes before Luis because the 0s tie and E < L |
| `?minAge=40&maxAge=50` | 200: Eva `{age: 46, totalOrders: 0, totalSpent: 0}` |
| `?minAge=39&maxAge=39` | 200: Ana only (the range is inclusive) |
| `?minAge=27&maxAge=38` | 200, `[]` |
| `?minAge=50` | 200, `[]` |
| `?minAge=40&maxAge=30` | 400, error on `minAge` |
| `?minAge=-1` | 400 |
| `?maxAge=abc` | 400 |

**Hints** (open only if you're stuck)
<details>
<summary>💡 Turning an age range into a date range</summary>

- age ≥ `minAge` ⇔ born **on or before** `today.AddYears(-minAge)`
- age ≤ `maxAge` ⇔ age < `maxAge + 1` ⇔ born **after** `today.AddYears(-(maxAge + 1))`

So: `c.Birthday <= bornOnOrBefore && c.Birthday > bornAfter`. Check it with `minAge=maxAge=39`: Ana, born 1987-01-13, has to be inside the range.
</details>

**Follow-ups:**
1. Why does `totalSpent` need `0` and not `null` for Eva? In plain SQL, what does `SUM` return over zero rows? (`NULL`. EF adds `COALESCE(SUM(...), 0)` for you.)
2. Write the same query as a `LEFT JOIN` + `GROUP BY`. What do you `COUNT`, so Eva gets 0 and not 1?
3. Look at the SQL EF generates for the subquery version: the `SUM` subquery appears **twice**, once in `SELECT` and again in `ORDER BY`. With a million orders, which version would you choose, and why?
4. Why is the `minAge > maxAge` check written in the action and not with `[Range]`? (`[Range]` checks one value; comparing two parameters needs code or `IValidatableObject` on a query object.)
5. Why does `ThenBy(name)` matter for hidden tests?

<details>
<summary>✅ Solution (subqueries, same approach as GET-6)</summary>

```csharp
public record CustomerOrderSummaryDto(string CustomerName, int Age, int TotalOrders, decimal TotalSpent);

[HttpGet("order-summary")]
[ProducesResponseType<List<CustomerOrderSummaryDto>>(StatusCodes.Status200OK)]
[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<List<CustomerOrderSummaryDto>>> GetOrderSummary(
    [FromQuery, Range(0, 150)] int minAge = 18,
    [FromQuery, Range(0, 150)] int maxAge = 120,
    CancellationToken ct = default)
{
    if (minAge > maxAge)
    {
        ModelState.AddModelError(nameof(minAge), "minAge must be less than or equal to maxAge.");
        return ValidationProblem(ModelState);                 // 400 ValidationProblemDetails
    }

    var today = DateTime.Today;
    var bornOnOrBefore = today.AddYears(-minAge);             // age >= minAge
    var bornAfter = today.AddYears(-(maxAge + 1));            // age <= maxAge

    var rows = await _db.Customers
        .AsNoTracking()
        .Where(c => c.Birthday <= bornOnOrBefore && c.Birthday > bornAfter)
        .Select(c => new
        {
            c.Name,
            c.Birthday,
            TotalOrders = _db.Orders.Count(o => o.CustomerId == c.Id),
            TotalSpent = _db.Orders
                .Where(o => o.CustomerId == c.Id &&
                            (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Delivered))
                .Sum(o => o.Total)                            // EF: COALESCE(SUM(...), 0.0)
        })
        .OrderByDescending(x => x.TotalSpent)
        .ThenBy(x => x.Name)
        .ToListAsync(ct);

    return rows
        .Select(r => new CustomerOrderSummaryDto(r.Name, AgeOn(r.Birthday, today), r.TotalOrders, r.TotalSpent))
        .ToList();
}
```
`AgeOn` is the same helper as in GET-6.
</details>

<details>
<summary>✅ Alternative: LEFT JOIN + GROUP BY (one pass over Orders)</summary>

Only the query changes; the validation and the date range stay the same.
```csharp
var rows = await (
        from c in _db.Customers
        where c.Birthday <= bornOnOrBefore && c.Birthday > bornAfter
        join o in _db.Orders on c.Id equals o.CustomerId into customerOrders
        from o in customerOrders.DefaultIfEmpty()             // LEFT JOIN: keeps Eva
        group o by new { c.Id, c.Name, c.Birthday } into g
        select new
        {
            g.Key.Name,
            g.Key.Birthday,
            TotalOrders = g.Count(o => o != null),            // not g.Count(): Eva's NULL row would count as 1
            TotalSpent = g.Sum(o => o != null &&
                                    (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Delivered)
                                    ? o.Total : 0m)
        })
    .OrderByDescending(x => x.TotalSpent)
    .ThenBy(x => x.Name)
    .AsNoTracking()
    .ToListAsync(ct);
```
SQL that EF Core 9 generates (shortened):
```sql
SELECT c.Name, c.Birthday,
       COUNT(CASE WHEN o.Id IS NOT NULL THEN 1 END)                                         AS TotalOrders,
       COALESCE(SUM(CASE WHEN o.Id IS NOT NULL AND o.Status IN ('Paid','Delivered') THEN o.Total ELSE 0.0 END), 0.0) AS TotalSpent
FROM Customers c
LEFT JOIN Orders o ON c.Id = o.CustomerId
WHERE c.Birthday <= @bornOnOrBefore AND c.Birthday > @bornAfter
GROUP BY c.Id, c.Name, c.Birthday
ORDER BY TotalSpent DESC, c.Name
```
This is **conditional aggregation**: `SUM(CASE WHEN ...)` works out several totals in one pass over the join. With two or more aggregates per parent, it's usually a better choice than one subquery per aggregate.
</details>

---

## GET-8 · Customer activity: orders + payments → 200 🟢 15 min

**Scenario:** The account team wants one line per customer, aged 30 or over, showing their orders and payments.

**Endpoint:** `GET /api/customers/activity` (no query parameters, no validation)

**Same skills as GET-6 and GET-7:** start from customers, keep customers with no orders or payments, filter on a birthday cutoff, calculate age in C#, sort with a tiebreaker, and return 200 `[]` when nothing matches. **New in GET-8:** counting from **two** child tables at once, and an **inclusive** age limit.

**Setup:** needs the payments from GET-6's setup. `Customer.Api` already seeds them.

**Requirements**
- Customers **30 or older**. This includes someone who is exactly 30, unlike GET-6's "older than 30".
- Body: a list of `{ customerName, age, totalOrders, totalPayments, amountPaid }`.
  - `totalOrders` = number of orders, any status.
  - `totalPayments` = number of payments. `amountPaid` = sum of their `Amount`.
  - A customer with nothing appears with `0`, `0`, `0`.
- Order by `amountPaid` descending, then `customerName`.
- No customer matches → **200 `[]`**.

**Test cases** (ages as of 2026-11-12; valid until 2027-01-13)
| Request | Expected |
|---|---|
| `GET /api/customers/activity` | 200, 2 items |
| item 1 | `{ customerName: "Ana", age: 39, totalOrders: 3, totalPayments: 3, amountPaid: 220 }` |
| item 2 | `{ customerName: "Eva", age: 46, totalOrders: 0, totalPayments: 0, amountPaid: 0 }` |
| check the list | no `Luis` (26) |
| check Ana | `totalOrders` is **3**, not 9, and `amountPaid` is **220**, not 660 |

**Follow-ups:**
1. GET-6 used `today.AddYears(-31)` for "older than 30". What's the cutoff for "30 or older", and why does it change by one?
2. The last test case: if you `LEFT JOIN` Orders **and** Payments and then `GROUP BY` the customer, Ana gets `totalOrders: 9`, `totalPayments: 9`, `amountPaid: 660`. Why? (Each of her 3 orders is paired with each of her 3 payments: 3 × 3 = 9 rows. This is the **fan-out** trap.)
3. Does `COUNT(DISTINCT o.Id)` fix the fan-out? (It fixes the counts but **not** `SUM(p.Amount)`. To fix the sum, total each child table separately, with a subquery or a derived table, before you join.)

<details>
<summary>✅ Solution</summary>

```csharp
public record CustomerActivityDto(string CustomerName, int Age, int TotalOrders, int TotalPayments, decimal AmountPaid);

[HttpGet("activity")]
[ProducesResponseType<List<CustomerActivityDto>>(StatusCodes.Status200OK)]
public async Task<ActionResult<List<CustomerActivityDto>>> GetActivity(CancellationToken ct)
{
    var today = DateTime.Today;
    var cutoff = today.AddYears(-30);                     // born on or before => 30 or older

    var rows = await _db.Customers
        .AsNoTracking()
        .Where(c => c.Birthday <= cutoff)
        .Select(c => new
        {
            c.Name,
            c.Birthday,
            // One subquery per child table: each one counts only its own rows, so there's no fan-out.
            TotalOrders   = _db.Orders.Count(o => o.CustomerId == c.Id),
            TotalPayments = _db.Payments.Count(p => p.CustomerId == c.Id),
            AmountPaid    = _db.Payments.Where(p => p.CustomerId == c.Id).Sum(p => p.Amount)
        })
        .OrderByDescending(x => x.AmountPaid)
        .ThenBy(x => x.Name)
        .ToListAsync(ct);

    return rows
        .Select(r => new CustomerActivityDto(r.Name, AgeOn(r.Birthday, today), r.TotalOrders, r.TotalPayments, r.AmountPaid))
        .ToList();
}
```
`AgeOn` is the same helper as in GET-6. Here the subqueries are the **right** tool, not just the easy one. A single `GROUP BY` over two joined child tables gives wrong numbers. I checked this against the database, and it returned 9 / 9 / 660 for Ana.
</details>

---

# PUT

## PUT-1 · Replace product → 204 / 400 / 404 🟢 15 min

**Endpoint:** `PUT /api/products/{id}`

**Requirements**
- Body is the full product: `{ name, sku, category, price, stock }`, with the same rules as POST-2.
- Not found → **404**. Invalid → **400**.
- Success → replace **all** fields, increment `Version`, return **204 No Content**.

**Test cases**
| Request | Expected |
|---|---|
| `PUT /api/products/1` with a valid body (`price: 999`) | 204, empty body |
| then `GET /api/products/1` | `price: 999`, `version: 2` |
| `PUT /api/products/999` | 404 |
| `PUT /api/products/1` with `name: ""` | 400 |

**Follow-ups:** 204 vs 200 with a body: when would you return the body? Is PUT idempotent if `Version++` runs every time? (The *resource* fields end up the same; the version is metadata. Good to discuss.)

<details>
<summary>✅ Solution</summary>

```csharp
[HttpPut("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Replace(int id, CreateProductRequest request)
{
    lock (_db.Sync)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (product is null) return NotFound();

        product.Name = request.Name;
        product.Sku = request.Sku;
        product.Category = request.Category;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Version++;

        return NoContent();
    }
}
```
</details>

---

## PUT-2 · Replace user (route id vs body id) → 200 / 400 / 404 🟢 15 min

**Endpoint:** `PUT /api/users/{id}`

**Requirements**
- Body: `{ id, name, email, phone? }`. `name` is required, `email` is required and must be a valid address, `phone` is optional.
- Route `id` ≠ body `id` → **400** (`title: "Id mismatch"`).
- Not found → **404**.
- Success → **200** with the updated user. Because it's PUT, an omitted `phone` becomes **null**.

**Test cases**
| Request | Expected |
|---|---|
| `PUT /api/users/2` `{"id":2,"name":"Luis P","email":"lp@mail.com"}` | 200, `phone: null` |
| `PUT /api/users/2` `{"id":1,...}` | 400 Id mismatch |
| `PUT /api/users/1` `{"id":1,"name":"A","email":"not-an-email"}` | 400, `errors.Email` |
| `PUT /api/users/99` `{"id":99,...}` | 404 |

**Follow-ups:** Should the body include the id at all? Why does `phone` become null here but not in PATCH-1? Which fields must never be overwritten (`createdAt`)?

<details>
<summary>✅ Solution</summary>

```csharp
public class ReplaceUserRequest
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Phone] public string? Phone { get; set; }
}

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly Db _db;
    public UsersController(Db db) => _db = db;

    [HttpPut("{id:int}")]
    public ActionResult<User> Replace(int id, ReplaceUserRequest request)
    {
        if (id != request.Id)
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                           title: "Id mismatch",
                           detail: $"Route id {id} does not match body id {request.Id}.");

        var user = _db.Users.FirstOrDefault(u => u.Id == id);
        if (user is null) return NotFound();

        user.Name = request.Name;
        user.Email = request.Email;
        user.Phone = request.Phone;      // full replace: null when it's omitted
        // CreatedAt is intentionally not touched

        return Ok(user);
    }
}
```
</details>

---

## PUT-3 · Upsert a setting → 201 (created) / 204 (updated) / 400 🟡 20 min

**Scenario:** The client owns the key, so PUT can create **or** update.

**Endpoints:** `PUT /api/settings/{key}`, `GET /api/settings/{key}`

**Requirements**
- `key` must match `^[a-z0-9-]{1,30}$` → otherwise **400**.
- Body: `{ "value": string }`. Required, max 200 characters.
- Key didn't exist → **201 Created** + `Location` + body `{ key, value }`.
- Key existed → **204**.
- GET → 200 `{ key, value }` or 404.

**Test cases**
| Request | Expected |
|---|---|
| `PUT /api/settings/language` `{"value":"es"}` | 201, `Location: /api/settings/language` |
| same request again | 204 |
| `PUT /api/settings/theme` `{"value":"light"}` | 204 (it exists in the seed) |
| `PUT /api/settings/Bad_Key!` | 400 |
| `GET /api/settings/language` | 200, `value: "es"` |
| `GET /api/settings/missing` | 404 |

**Follow-ups:** Why is PUT allowed to create but POST is normally used instead? Is the repeated PUT idempotent even though the status code changes from 201 to 204? (Yes. Idempotency is about server state, not identical responses.)

<details>
<summary>✅ Solution</summary>

```csharp
public class SettingRequest
{
    [Required, StringLength(200)] public string Value { get; set; } = "";
}

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private static readonly Regex KeyFormat = new("^[a-z0-9-]{1,30}$", RegexOptions.Compiled);
    private readonly Db _db;
    public SettingsController(Db db) => _db = db;

    [HttpGet("{key}")]
    public IActionResult Get(string key)
        => _db.Settings.TryGetValue(key, out var value) ? Ok(new { key, value }) : NotFound();

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult Upsert(string key, SettingRequest request)
    {
        if (!KeyFormat.IsMatch(key))
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                           title: "Invalid key",
                           detail: "Key must match ^[a-z0-9-]{1,30}$");

        lock (_db.Sync)
        {
            var existed = _db.Settings.ContainsKey(key);
            _db.Settings[key] = request.Value;

            return existed
                ? NoContent()
                : CreatedAtAction(nameof(Get), new { key }, new { key, value = request.Value });
        }
    }
}
```
</details>

---

## PUT-4 · Update price: validation (400) vs business rule (422) 🟡 20 min

**Endpoint:** `PUT /api/products/{id}/price`

**Requirements**
- Body: `{ "price": decimal }`. A value ≤ 0 → **400** (input validation).
- The new price can't be more than **50% above** the current price → **422** ProblemDetails, with `currentPrice` and `maxAllowedPrice` in the extensions.
- Not found → **404**. Success → **204**.

**Test cases** (Laptop costs 1200, so the max is 1800)
| Request | Expected |
|---|---|
| `PUT /api/products/1/price` `{"price":1500}` | 204 |
| `{"price":1800}` | 204 (the limit is inclusive) |
| `{"price":2000}` | 422, `maxAllowedPrice: 1800` |
| `{"price":0}` | 400 |
| `PUT /api/products/999/price` `{"price":10}` | 404 |

**Follow-ups:** Explain 400 vs 422 in one sentence. Where does this rule belong in a CQRS app like this repo? (In the command handler or domain model, throwing a `BusinessRuleException` that middleware maps to 422.)

<details>
<summary>✅ Solution</summary>

```csharp
public class UpdatePriceRequest
{
    [Range(typeof(decimal), "0.01", "1000000")] public decimal Price { get; set; }
}

[HttpPut("{id:int}/price")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
public IActionResult UpdatePrice(int id, UpdatePriceRequest request)
{
    lock (_db.Sync)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (product is null) return NotFound();

        var maxAllowed = product.Price * 1.5m;
        if (request.Price > maxAllowed)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Price increase too large",
                Detail = "A price can increase at most 50% in one update."
            };
            problem.Extensions["currentPrice"] = product.Price;
            problem.Extensions["maxAllowedPrice"] = maxAllowed;
            return UnprocessableEntity(problem);
        }

        product.Price = request.Price;
        product.Version++;
        return NoContent();
    }
}
```
</details>

---

## PUT-5 · Optimistic concurrency with If-Match → 204 / 412 / 428 🔴 30 min

**Scenario:** Two admins edit the same product. The second save must not silently overwrite the first (the "lost update" problem).

**Endpoint:** `PUT /api/products/{id}` (extends PUT-1; ETag format from GET-5: `"{Version}"`)

**Requirements**
- Not found → **404**.
- No `If-Match` header → **428 Precondition Required**.
- `If-Match` ≠ the current ETag → **412 Precondition Failed**, and tell the client the current ETag.
- Match (or `*`) → update, `Version++`, **204** with the new `ETag` header.

**Test cases**
| Request | Expected |
|---|---|
| `PUT /api/products/1` + `If-Match: "1"` | 204, `ETag: "2"` |
| same again with `If-Match: "1"` | 412 (the version is now 2) |
| `PUT /api/products/1` + `If-Match: "2"` | 204, `ETag: "3"` |
| `PUT /api/products/1` without the header | 428 |
| `PUT /api/products/999` + `If-Match: "1"` | 404 |

**Follow-ups:** How does EF Core do this? (A `[Timestamp] byte[] RowVersion` or `IsConcurrencyToken()`, then catch `DbUpdateConcurrencyException` and return 409 or 412.) 409 vs 412? Why is 428 useful?

<details>
<summary>✅ Solution</summary>

```csharp
[HttpPut("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status412PreconditionFailed)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status428PreconditionRequired)]
public IActionResult Replace(int id, CreateProductRequest request)
{
    lock (_db.Sync)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (product is null) return NotFound();

        var ifMatch = Request.Headers.IfMatch.ToString();
        if (string.IsNullOrEmpty(ifMatch))
            return Problem(statusCode: StatusCodes.Status428PreconditionRequired,
                           title: "If-Match header is required");

        var currentEtag = $"\"{product.Version}\"";
        if (ifMatch != "*" && ifMatch != currentEtag)
            return Problem(statusCode: StatusCodes.Status412PreconditionFailed,
                           title: "The product was modified by someone else",
                           detail: $"Current ETag is {currentEtag}. GET the latest version and retry.");

        product.Name = request.Name;
        product.Sku = request.Sku;
        product.Category = request.Category;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Version++;

        Response.Headers.ETag = $"\"{product.Version}\"";
        return NoContent();
    }
}
```
</details>

---

# PATCH

## PATCH-1 · Partial user update (only the fields sent) → 200 / 400 / 404 🟢 15 min

**Endpoint:** `PATCH /api/users/{id}` (`Content-Type: application/json`)

**Requirements**
- Body may contain any of `name`, `email`, `phone`. **Only the fields you send change.**
- Empty body `{}` → **400** (`title: "No fields to update"`).
- Invalid email → **400**. Not found → **404**.
- Success → **200** with the updated user.

**Test cases**
| Request | Expected |
|---|---|
| `PATCH /api/users/2` `{"email":"new@mail.com"}` | 200, email changed, **phone unchanged** |
| `PATCH /api/users/1` `{"phone":"+51 900 000 000"}` | 200, name and email unchanged |
| `PATCH /api/users/1` `{}` | 400 |
| `PATCH /api/users/1` `{"email":"bad"}` | 400 |
| `PATCH /api/users/99` `{"name":"X"}` | 404 |

**Follow-ups:** With nullable DTO properties, how do you tell "not sent" apart from "set to null"? (You can't. Use JSON Patch (PATCH-4), JSON Merge Patch, or an `Optional<T>` wrapper.) PUT vs PATCH in one sentence.

<details>
<summary>✅ Solution</summary>

```csharp
public class PatchUserRequest
{
    [StringLength(100, MinimumLength = 1)] public string? Name { get; set; }  // null = not sent
    [EmailAddress] public string? Email { get; set; }                          // null passes validation
    [Phone] public string? Phone { get; set; }
}

[HttpPatch("{id:int}")]
[ProducesResponseType<User>(StatusCodes.Status200OK)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public ActionResult<User> Patch(int id, PatchUserRequest request)
{
    if (request.Name is null && request.Email is null && request.Phone is null)
        return Problem(statusCode: StatusCodes.Status400BadRequest, title: "No fields to update");

    var user = _db.Users.FirstOrDefault(u => u.Id == id);
    if (user is null) return NotFound();

    if (request.Name is not null)  user.Name = request.Name;
    if (request.Email is not null) user.Email = request.Email;
    if (request.Phone is not null) user.Phone = request.Phone;

    return Ok(user);
}
```
</details>

---

## PATCH-2 · Change order status → 200 / 400 / 404 🟢 15 min

**Endpoint:** `PATCH /api/orders/{id}/status`

**Requirements**
- Body: `{ "status": "Shipped" }` (enum by name). Missing status → **400**. Unknown value → **400**.
- Not found → **404**.
- Success → **200** `{ id, status }`. Allow any transition for now; PATCH-3 adds the rules.

**Test cases**
| Request | Expected |
|---|---|
| `PATCH /api/orders/1/status` `{"status":"Paid"}` | 200, `status: "Paid"` |
| `{"status":"Flying"}` | 400 |
| `{}` | 400 |
| `PATCH /api/orders/999/status` `{"status":"Paid"}` | 404 |

**Follow-ups:** Why does `status` need to be `OrderStatus?` with `[Required]` and not just `OrderStatus`? (A missing non-nullable enum becomes `Pending` (0) and passes validation.) What makes `"Flying"` return a 400? (`JsonStringEnumConverter` throws, and model binding turns that into a 400.)

<details>
<summary>✅ Solution</summary>

```csharp
public class ChangeStatusRequest
{
    [Required] public OrderStatus? Status { get; set; }
}

[HttpPatch("{id:int}/status")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult ChangeStatus(int id, ChangeStatusRequest request)
{
    var order = _db.Orders.FirstOrDefault(o => o.Id == id);
    if (order is null) return NotFound();

    order.Status = request.Status!.Value;
    return Ok(new { order.Id, order.Status });
}
```
</details>

---

## PATCH-3 · Status state machine → 200 / 409 🟡 20 min

**Endpoint:** `PATCH /api/orders/{id}/status` (extends PATCH-2)

**Allowed transitions**
```
Pending  -> Paid | Cancelled
Paid     -> Shipped | Cancelled
Shipped  -> Delivered
Delivered, Cancelled -> (final, nothing allowed)
```

**Requirements**
- Invalid transition → **409** ProblemDetails, with `currentStatus` and `allowedTransitions` in the extensions.
- Valid → **200**.

**Test cases**
| Request | Expected |
|---|---|
| order 1 (Pending) → `Paid` | 200 |
| order 1 (Pending) → `Delivered` | 409, `allowedTransitions: ["Paid","Cancelled"]` |
| order 3 (Paid) → `Shipped` | 200 |
| order 4 (Delivered) → `Pending` | 409, `allowedTransitions: []` |
| order 4 (Delivered) → `Delivered` | 409 |

**Follow-ups:** Where should the state machine live? (In the domain entity, as `order.TransitionTo(status)`, not in the controller.) 409 vs 422 here? Would you use separate action endpoints (`POST /orders/1/ship`) instead?

<details>
<summary>✅ Solution</summary>

```csharp
private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
{
    [OrderStatus.Pending]   = new[] { OrderStatus.Paid, OrderStatus.Cancelled },
    [OrderStatus.Paid]      = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
    [OrderStatus.Shipped]   = new[] { OrderStatus.Delivered },
    [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
    [OrderStatus.Cancelled] = Array.Empty<OrderStatus>(),
};

[HttpPatch("{id:int}/status")]
public IActionResult ChangeStatus(int id, ChangeStatusRequest request)
{
    lock (_db.Sync)
    {
        var order = _db.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return NotFound();

        var target = request.Status!.Value;
        var allowed = AllowedTransitions[order.Status];

        if (!allowed.Contains(target))
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Invalid status transition",
                Detail = $"Cannot change order {id} from {order.Status} to {target}."
            };
            problem.Extensions["currentStatus"] = order.Status.ToString();
            problem.Extensions["allowedTransitions"] = allowed.Select(s => s.ToString());
            return Conflict(problem);
        }

        order.Status = target;
        return Ok(new { order.Id, order.Status });
    }
}
```
</details>

---

## PATCH-4 · JSON Patch document (RFC 6902) → 200 / 400 / 404 / 415 🟡 25 min

**Endpoint:** `PATCH /api/products/{id}` with `Content-Type: application/json-patch+json`

**Setup**
```bash
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
```
```csharp
// Program.cs: JsonPatchDocument needs Newtonsoft, which replaces System.Text.Json for all controllers
builder.Services.AddControllers()
    .AddNewtonsoftJson(o => o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));
```
(.NET 10 also ships a System.Text.Json version: `Microsoft.AspNetCore.JsonPatch.SystemTextJson`.)

**Requirements**
- Patchable fields: `name`, `category`, `price`, `stock`. The same validation rules as POST-2 apply **after** the patch.
- Unknown path (for example `/sku` or `/id`) or an invalid operation → **400**.
- Result is invalid after the patch (for example `price: -1`) → **400** ValidationProblem.
- Wrong `Content-Type` → **415**. Not found → **404**.
- Success → increment `Version` and return **200** with the product.

**Test cases**
| Body (`application/json-patch+json`) | Expected |
|---|---|
| `[{"op":"replace","path":"/price","value":999}]` | 200, `price: 999` |
| `[{"op":"replace","path":"/name","value":"Pro"},{"op":"replace","path":"/stock","value":0}]` | 200 |
| `[{"op":"replace","path":"/sku","value":"X"}]` | 400 (path not in the model) |
| `[{"op":"replace","path":"/price","value":-1}]` | 400 |
| `PATCH /api/products/999` | 404 |
| same body with `Content-Type: application/json` | 415 |

**Follow-ups:** Why patch a dedicated model and not the entity? (So the client can't change `/id` or `/isDeleted`.) What do JSON Patch and JSON Merge Patch (RFC 7386) each look like?

<details>
<summary>✅ Solution</summary>

```csharp
using Microsoft.AspNetCore.JsonPatch;

public class ProductPatchModel
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required] public string Category { get; set; } = "";
    [Range(typeof(decimal), "0.01", "1000000")] public decimal Price { get; set; }
    [Range(0, int.MaxValue)] public int Stock { get; set; }
}

[HttpPatch("{id:int}")]
[Consumes("application/json-patch+json")]          // anything else -> 415
[ProducesResponseType<Product>(StatusCodes.Status200OK)]
[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Patch(int id, [FromBody] JsonPatchDocument<ProductPatchModel> patch)
{
    lock (_db.Sync)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (product is null) return NotFound();

        var model = new ProductPatchModel
        {
            Name = product.Name, Category = product.Category,
            Price = product.Price, Stock = product.Stock
        };

        patch.ApplyTo(model, ModelState);           // bad path or op -> ModelState error
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (!TryValidateModel(model)) return ValidationProblem(ModelState);  // DataAnnotations after the patch

        product.Name = model.Name;
        product.Category = model.Category;
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.Version++;

        return Ok(product);
    }
}
```
</details>

---

## PATCH-5 · Concurrent stock adjustment (thread safety) → 200 / 400 / 409 🔴 30 min

**Scenario:** This is the same domain as `ProductInStockUpdateStockEventHandler` in this repo. Many checkouts happen at the same time, and stock must **never** go negative.

**Endpoint:** `PATCH /api/products/{id}/stock`

**Requirements**
- Body: `{ "delta": int }`, from -1000 to 1000. `delta == 0` → **400**.
- Stock after the change would be < 0 → **409** (`title: "Insufficient stock"`), and the stock is **not** changed.
- Not found → **404**. Success → **200** `{ productId, stock }`.
- **Hidden test:** 20 parallel requests with `delta: -1` on product 3 (stock 5) → exactly **5×200, 15×409**, and the final stock is **0**.

**Test cases**
| Request | Expected |
|---|---|
| `PATCH /api/products/3/stock` `{"delta":-2}` | 200, `stock: 3` |
| `{"delta":-10}` on product 3 | 409, stock unchanged |
| `{"delta":5}` on product 3 | 200, `stock: 10` |
| `{"delta":0}` | 400 |
| 20 × parallel `{"delta":-1}` on product 3 | 5 × 200, 15 × 409, final 0 |

**Follow-ups:** Why does a check-then-update without a lock fail under load? How would you do this with SQL or EF? (An atomic `UPDATE ... SET Stock = Stock + @d WHERE Id = @id AND Stock + @d >= 0`, then check the number of rows affected, or a RowVersion concurrency token with retry.) PATCH is not idempotent here: is that OK? Should retries use an Idempotency-Key (POST-5)?

<details>
<summary>✅ Solution</summary>

```csharp
public class AdjustStockRequest
{
    [Range(-1000, 1000)] public int Delta { get; set; }
}

[HttpPatch("{id:int}/stock")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
public IActionResult AdjustStock(int id, AdjustStockRequest request)
{
    if (request.Delta == 0)
        return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Delta cannot be zero");

    lock (_db.Sync)   // read + check + write as one atomic step
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (product is null) return NotFound();

        if (product.Stock + request.Delta < 0)
            return Problem(statusCode: StatusCodes.Status409Conflict,
                           title: "Insufficient stock",
                           detail: $"Product {id} has {product.Stock}, requested {-request.Delta}.");

        product.Stock += request.Delta;
        product.Version++;
        return Ok(new { productId = product.Id, stock = product.Stock });
    }
}
```

The same thing with EF Core 7+ is one atomic SQL statement, with no lock and no race:
```csharp
var affected = await _db.Products
    .Where(p => p.Id == id && p.Stock + delta >= 0)
    .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock + delta), ct);

if (affected == 0)
    return await _db.Products.AnyAsync(p => p.Id == id, ct) ? Conflict() : NotFound();
```
</details>

---

# DELETE

## DELETE-1 · Delete product → 204 / 404 🟢 15 min

**Endpoint:** `DELETE /api/products/{id}` (hard delete: remove it from the list)

**Requirements**
- Exists → remove it → **204 No Content**.
- Doesn't exist → **404**.

**Test cases**
| Request | Expected |
|---|---|
| `DELETE /api/products/2` | 204, empty body |
| `GET /api/products/2` | 404 |
| `DELETE /api/products/2` again | 404 |
| `DELETE /api/products/abc` | 404 |

**Follow-ups:** 204 vs 200 with the deleted object vs 202? Should a DELETE body ever be used?

<details>
<summary>✅ Solution</summary>

```csharp
[HttpDelete("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Delete(int id)
{
    lock (_db.Sync)
    {
        var removed = _db.Products.RemoveAll(p => p.Id == id);
        return removed > 0 ? NoContent() : NotFound();
    }
}
```
</details>

---

## DELETE-2 · Idempotent delete: remove an item from a cart → always 204 🟢 15 min

**Endpoint:** `DELETE /api/carts/{cartId}/items/{productId}`

**Requirements**
- Cart doesn't exist → **404**.
- Cart exists → remove the product if it's there → **204**, **even if the item wasn't in the cart**.

**Test cases** (seed: cart 1 has products 1 and 2)
| Request | Expected |
|---|---|
| `DELETE /api/carts/1/items/2` | 204 |
| `DELETE /api/carts/1/items/2` again | 204 (idempotent) |
| `DELETE /api/carts/1/items/999` | 204 |
| `DELETE /api/carts/99/items/1` | 404 |

**Follow-ups:** Compare this with DELETE-1, which returns 404 on the second call. Both are idempotent. Which would you choose for a public API, and why? (Retries after a network timeout are a good argument for always returning 204.)

<details>
<summary>✅ Solution</summary>

```csharp
[ApiController]
[Route("api/carts")]
public class CartsController : ControllerBase
{
    private readonly Db _db;
    public CartsController(Db db) => _db = db;

    [HttpDelete("{cartId:int}/items/{productId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoveItem(int cartId, int productId)
    {
        lock (_db.Sync)
        {
            if (!_db.Carts.TryGetValue(cartId, out var items)) return NotFound();

            items.Remove(productId);     // no-op if it isn't there
            return NoContent();
        }
    }
}
```
</details>

---

## DELETE-3 · Block delete when there are dependents → 204 / 409 🟡 20 min

**Endpoint:** `DELETE /api/customers/{id}`

**Requirements**
- Not found → **404**.
- Customer has orders → **409** ProblemDetails (`title: "Customer has orders"`) with `orderCount` in the extensions. **Don't** delete.
- No orders → delete → **204**.

**Test cases**
| Request | Expected |
|---|---|
| `DELETE /api/customers/3` | 204 |
| `DELETE /api/customers/1` | 409, `orderCount: 3` |
| `DELETE /api/customers/999` | 404 |
| `GET /api/customers/1/orders` after the 409 | 200, still 3 orders |

**Follow-ups:** Cascade delete vs restrict in EF Core (`OnDelete(DeleteBehavior.Restrict)`)? Would you offer `?force=true`? Who should be allowed to use it?

<details>
<summary>✅ Solution</summary>

```csharp
[HttpDelete("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
public IActionResult Delete(int id)
{
    lock (_db.Sync)
    {
        var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
        if (customer is null) return NotFound();

        var orderCount = _db.Orders.Count(o => o.CustomerId == id);
        if (orderCount > 0)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Customer has orders",
                Detail = $"Customer {id} cannot be deleted while orders exist."
            };
            problem.Extensions["orderCount"] = orderCount;
            return Conflict(problem);
        }

        _db.Customers.Remove(customer);
        return NoContent();
    }
}
```
</details>

---

## DELETE-4 · Soft delete + 410 Gone 🟡 20 min

**Scenario:** For audits, products are never physically removed.

**Endpoints:** `DELETE /api/products/{id}`, `GET /api/products/{id}`

**Requirements**
- DELETE on an active product → set `IsDeleted = true` → **204**.
- DELETE on an already-deleted product → **410 Gone**.
- GET on a deleted product → **410 Gone**, not 404, because it existed before.
- GET or DELETE on an id that never existed → **404**.
- `GET /api/products` (the list) leaves deleted products out.

**Test cases**
| Request | Expected |
|---|---|
| `DELETE /api/products/2` | 204 |
| `GET /api/products/2` | 410 |
| `DELETE /api/products/2` again | 410 |
| `GET /api/products` | 2 items (no Mouse) |
| `DELETE /api/products/999` | 404 |

**Follow-ups:** 410 vs 404: does 410 leak information? In EF Core, how do you hide deleted rows everywhere? (A global query filter, `HasQueryFilter(p => !p.IsDeleted)`, and `IgnoreQueryFilters()` to see them.) How would you add `POST /api/products/{id}/restore`?

<details>
<summary>✅ Solution</summary>

```csharp
[HttpGet("{id:int}")]
[ProducesResponseType<Product>(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status410Gone)]
public ActionResult<Product> GetById(int id)
{
    var product = _db.Products.FirstOrDefault(p => p.Id == id);
    if (product is null) return NotFound();
    if (product.IsDeleted) return Problem(statusCode: StatusCodes.Status410Gone, title: "Product was deleted");
    return product;
}

[HttpDelete("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status410Gone)]
public IActionResult SoftDelete(int id)
{
    lock (_db.Sync)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id);
        if (product is null) return NotFound();
        if (product.IsDeleted) return Problem(statusCode: StatusCodes.Status410Gone, title: "Product was already deleted");

        product.IsDeleted = true;
        product.Version++;
        return NoContent();
    }
}
```
</details>

---

## DELETE-5 · API-key protected delete → 401 / 403 / 204 🔴 30 min

**Scenario:** Only admins can delete products. Callers send `X-Api-Key`. Build it as a **reusable attribute**, not as `if` checks inside every action.

**Keys** (hard-code them for the exercise): `reader-key` → role `reader`, `admin-key` → role `admin`.

**Endpoint:** `DELETE /api/products/{id}` decorated with `[RequireApiKey("admin")]`

**Requirements**
- Missing or unknown key → **401**, with the header `WWW-Authenticate: ApiKey`.
- Valid key with the wrong role → **403** ProblemDetails.
- Admin key → normal delete → **204** or **404**.
- Auth runs **before** the action, so a 401 or 403 never reveals whether the product exists.

**Test cases**
| Request | Expected |
|---|---|
| `DELETE /api/products/1`, no header | 401, `WWW-Authenticate: ApiKey` |
| `X-Api-Key: wrong` | 401 |
| `X-Api-Key: reader-key` | 403 |
| `X-Api-Key: admin-key` | 204 |
| `X-Api-Key: reader-key` on `/api/products/999` | 403 (not 404) |
| `X-Api-Key: admin-key` on `/api/products/999` | 404 |

**Follow-ups:** 401 vs 403 in one sentence. Why does `return Forbid();` throw here? (`ForbidResult` and `ChallengeResult` need an authentication scheme to be registered.) How would you do this properly? (A custom `AuthenticationHandler<ApiKeyOptions>` that sets claims, then `[Authorize(Roles = "admin")]` or a policy.) Where should the keys live? (Key Vault or configuration, stored hashed, and compared with `CryptographicOperations.FixedTimeEquals`.)

<details>
<summary>✅ Solution</summary>

```csharp
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private static readonly Dictionary<string, string> KeyToRole = new()
    {
        ["reader-key"] = "reader",
        ["admin-key"]  = "admin",
    };

    private readonly string _requiredRole;
    public RequireApiKeyAttribute(string requiredRole) => _requiredRole = requiredRole;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var key = context.HttpContext.Request.Headers["X-Api-Key"].ToString();

        if (string.IsNullOrEmpty(key) || !KeyToRole.TryGetValue(key, out var role))
        {
            context.HttpContext.Response.Headers.WWWAuthenticate = "ApiKey";
            context.Result = new UnauthorizedResult();                         // 401
            return;
        }

        if (role != _requiredRole)
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = $"Role '{_requiredRole}' is required."
            }) { StatusCode = StatusCodes.Status403Forbidden };                // 403, no auth scheme needed
        }
    }
}

// ProductsController
[HttpDelete("{id:int}")]
[RequireApiKey("admin")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Delete(int id)
{
    lock (_db.Sync)
    {
        var removed = _db.Products.RemoveAll(p => p.Id == id);
        return removed > 0 ? NoContent() : NotFound();
    }
}
```
</details>

---

# Bonus: write the test (some platforms ask for this)

```bash
dotnet new xunit -n MockApi.Tests
dotnet add MockApi.Tests reference MockApi
dotnet add MockApi.Tests package Microsoft.AspNetCore.Mvc.Testing
```

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

public class OrderPayTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public OrderPayTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Theory]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(2, HttpStatusCode.PaymentRequired)]
    [InlineData(3, HttpStatusCode.Conflict)]
    [InlineData(999, HttpStatusCode.NotFound)]
    public async Task Pay_returns_expected_status(int orderId, HttpStatusCode expected)
    {
        var response = await _client.PostAsync($"/api/orders/{orderId}/pay", content: null);
        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Create_product_returns_201_with_location()
    {
        var response = await _client.PostAsJsonAsync("/api/products",
            new { name = "Chair", sku = "CHR-001", category = "Furniture", price = 80, stock = 3 });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Parallel_stock_decrements_never_go_negative()
    {
        var tasks = Enumerable.Range(0, 20).Select(_ =>
            _client.PatchAsJsonAsync("/api/products/3/stock", new { delta = -1 }));
        var results = await Task.WhenAll(tasks);

        Assert.Equal(5,  results.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Equal(15, results.Count(r => r.StatusCode == HttpStatusCode.Conflict));
    }
}
```
> The `Db` singleton is shared by every test in the same fixture. Keep the tests independent: use different ids, or create a new factory per test.

---

# Self-review checklist (after each exercise)

- [ ] Correct verb, and a route with a constraint (`{id:int}`, `{id:guid}`)
- [ ] Correct success code: 200, 201 + Location, 202 + Location, or 204
- [ ] Correct error code: 400 vs 404 vs 409 vs 412 vs 422
- [ ] Errors use **ProblemDetails** (`Problem(...)`, `ValidationProblem(...)`)
- [ ] Validation is declarative (DataAnnotations + `[ApiController]`) where possible
- [ ] Return a DTO, not the entity, when the entity has internal fields
- [ ] `[ProducesResponseType]` for every status code the action can return
- [ ] Check-then-write is atomic (a lock here; a transaction, unique index, or concurrency token in a real DB)
- [ ] Business rules belong in the handler or domain, not the controller (say this out loud in the interview)
- [ ] Finished inside the time box
