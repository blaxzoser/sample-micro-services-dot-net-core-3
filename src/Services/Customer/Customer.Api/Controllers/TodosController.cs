using Customer.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
[Route("todos")]
public class TodosController : ControllerBase
{
    private static readonly Todo[] SampleTodos =
    {
        new(1, "Walk the dog"),
        new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
        new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
        new(4, "Clean the bathroom"),
        new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
    };

    // GET /todos
    [HttpGet]
    public ActionResult<IEnumerable<Todo>> GetAll()
    {
        return Ok(SampleTodos);
    }

    // GET /todos/{id}
    [HttpGet("{id:int}")]
    public ActionResult<Todo> Get(int id)
    {
        var todo = SampleTodos.FirstOrDefault(t => t.Id == id);

        if (todo is null)
        {
            return NotFound();
        }

        return Ok(todo);
    }
}
