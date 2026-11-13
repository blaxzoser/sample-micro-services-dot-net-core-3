using Customer.Api.DTO;
using Customer.Api.Models;

namespace Customer.Api.Controllers;

public record CustomerDto(int Id, string Name, decimal Balance);

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db) => _db = db;

    // GET /api/customers/{customerId}
    // 200 with the customer, 404 ProblemDetails if it doesn't exist.
    // ":int" makes /api/customers/abc a 404 instead of a 400 from model binding.
    [HttpGet("{customerId:int}")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(int customerId, CancellationToken ct)
    {
        // FirstOrDefaultAsync, not SingleAsync: a missing row gives null (-> 404), not an exception (-> 500).
        var customer = await _db.Customers
            .AsNoTracking()
            .Where(c => c.Id == customerId)
            .Select(c => new CustomerDto(c.Id, c.Name, c.Balance))
            .FirstOrDefaultAsync(ct);

        if (customer is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                           title: "Customer not found",
                           detail: $"Customer {customerId} does not exist.");
        }

        return customer;   // implicit 200 via ActionResult<T>
    }

    // GET-6: GET /api/customers/over-30
    // Customers older than 30 (age 31+) with how many payments each has, ordered by name.
    // 200 with [] when nobody matches: an empty filter result isn't "not found".
    [HttpGet("over-30")]
    [ProducesResponseType<List<CustomerByAgeDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerByAgeDto>>> GetByAge(CancellationToken ct)
    {
        // Read the clock once, so every row uses the same "today".
        var today = DateTime.Today;

        // CalculateAge can't go in Where (EF can't translate it to SQL), so filter on a cutoff date:
        // born on or before today minus 31 years = 31 or older = older than 30.
        // A column compared with a constant lets SQL Server use an index on Birthday.
        var cutoff = today.AddYears(-31);

        var result = await _db.Customers
            .AsNoTracking()
            .Where(c => c.Birthday <= cutoff)
            .OrderBy(c => c.Name)
            .Select(c => new CustomerByAgeDto
            {
                CustomerName = c.Name,
                // Runs in C# after the query: EF allows that in the final Select only.
                Age = CalculateAge(c.Birthday, today),
                // Correlated COUNT subquery, starting from the customer, so a customer
                // with no payments still appears with 0. An inner join on payments would drop them.
                TotalPayments = _db.Payments.Count(p => p.CustomerId == c.Id)
            })
            .ToListAsync(ct);

        return result;
    }

    static int CalculateAge(DateTime birthday, DateTime today)
    {
        // Plain year subtraction over-counts by one before this year's birthday has passed.
        var age = today.Year - birthday.Year;
        if (birthday.Date > today.AddYears(-age))
            age--;
        return age;
    }


    [HttpGet("order-summary")]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetSummary(CancellationToken ct,[FromQuery]int minAge = 18, [FromQuery]int maxAge = 120)
    {

        if (minAge is < 0 or > 150 || maxAge is < 0 or > 150)
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                         title: "Customer range bad");

        if(minAge > maxAge)
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                        title: "Customer range bad");

        var today = DateTime.Today;
        var minDate = today.AddYears(-minAge);
        var maxDate = today.AddYears(-(maxAge +1));

        var  results = await  _db.Customers
            .AsNoTracking()
            .Where( c => c.Birthday <= minDate && c.Birthday > maxDate)
            .Select(x => new OrderSummaryDto()
            {
                CustomerName = x.Name,
                Age = CalculateDb(today, x.Birthday),
                TotalOrders = _db.Orders.Count(p => p.CustomerId == x.Id),
                TotalSpent = _db.Orders.Where(p => p.CustomerId == x.Id && (p.Status == OrderStatus.Paid || p.Status == OrderStatus.Delivered))
                .Sum( p => p.Total)
            })
            .OrderByDescending(x => x.TotalSpent)
            .ThenBy(x => x.CustomerName)
            .ToListAsync(ct); 

         return results;
    }

    static int CalculateDb(DateTime now, DateTime bd)
    {
        return now.Year - bd.Year; 
    }

}
