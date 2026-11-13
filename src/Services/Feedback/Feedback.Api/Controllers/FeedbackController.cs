namespace Feedback.Api.Controllers;

[ApiController]
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly AppDbContext _db;

    public FeedbackController(AppDbContext db) => _db = db;

    // POST-1 · Submit feedback -> 200 / 400   (doc/aspnet-mock-interview-exercises.md)
    //
    // TODO: implement the exercise here.
    //   1. Add the validation rules to FeedbackRequest.
    //   2. Map the request to a FeedbackEntry, add it to _db.Feedbacks and save.
    //   3. Return 200 OK with a FeedbackResponse { id, message, rating, receivedAt }.
    // Run the tests in Feedback.Api.Tests to check your work.
    [HttpPost]
    public async Task<ActionResult<FeedbackResponse>> Create([FromBody]FeedbackRequest request)
    {
        // FIX: IsNullOrWhiteSpace (was IsNullOrEmpty) so "   " is rejected too.
        // FIX: the tests read errors.Message / errors.Rating, so use ValidationProblem
        //      (same status/title/detail as your Problem(...) calls, plus an "errors" map).
        if (string.IsNullOrWhiteSpace(request.Message))
               return ValidationProblem(new ValidationProblemDetails(
                   new Dictionary<string, string[]> { ["Message"] = new[] { "Message is required" } })
               {
                   Status = StatusCodes.Status400BadRequest,
                   Title = "Fields Missing",
                   Detail = "Problem with message, it's requited"
               });

        if (request.Message.Length > 500)
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]> { ["Message"] = new[] { "Message must be at most 500 characters" } })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Error Field",
                Detail = "message should be less 500, it's requited"
            });

        // FIX: was `&&` (never true); out of range means < 1 OR > 5.
        if (request.Rating < 1 || request.Rating > 5)
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]> { ["Rating"] = new[] { "Rating must be between 1 and 5" } })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Range invalid",
                Detail = "message should be 1 to 5, it's requited"
            });

        // FIX: DateTime.UtcNow (was DateTime.Now); the model documents ReceivedAt as UTC.
         var newFeedBack = await _db.Feedbacks.AddAsync(new FeedbackEntry() { Message = request.Message, Rating = request.Rating, ReceivedAt = DateTime.UtcNow });

        await _db.SaveChangesAsync();

        // FIX: removed stray Task.FromResult(...) / "});" that broke the build; return Ok(...).
        return Ok(new FeedbackResponse(newFeedBack.Entity.Id, newFeedBack.Entity.Message, newFeedBack.Entity.Rating, newFeedBack.Entity.ReceivedAt));
    }

    // ------------------------------------------------------------------------------------
    // CLAUDE'S ALTERNATIVE (commented out, for comparison with the solution above)
    //
    // Idea: declare the rules on FeedbackRequest with DataAnnotations. [ApiController]
    // validates the model before the action runs and returns 400 with an "errors" map
    // (errors.Message / errors.Rating) by itself, so the action only does the happy path.
    //
    // 1) DTO/FeedbackRequest.cs
    //
    //    public class FeedbackRequest
    //    {
    //        [Required]            // also rejects "" and "   " (AllowEmptyStrings = false)
    //        [StringLength(500)]   // max 500 characters
    //        public string Message { get; set; } = "";
    //
    //        [Range(1, 5)]         // 1..5 inclusive
    //        public int Rating { get; set; }
    //    }
    //
    // 2) Controller action (no manual if/ValidationProblem needed):
    //
    //    [HttpPost]
    //    public async Task<ActionResult<FeedbackResponse>> Create([FromBody] FeedbackRequest request)
    //    {
    //        var entry = new FeedbackEntry
    //        {
    //            Message = request.Message,
    //            Rating = request.Rating,
    //            ReceivedAt = DateTime.UtcNow
    //        };
    //
    //        _db.Feedbacks.Add(entry);
    //        await _db.SaveChangesAsync();
    //
    //        return Ok(new FeedbackResponse(entry.Id, entry.Message, entry.Rating, entry.ReceivedAt));
    //    }
    //
    // Why it is better: rules live in one place (the DTO), are reusable by other actions,
    // report ALL invalid fields at once (manual ifs stop at the first), and the action stays short.
    // Verified: this version passes the same 17 tests in Feedback.Api.Tests.
    // ------------------------------------------------------------------------------------
}
