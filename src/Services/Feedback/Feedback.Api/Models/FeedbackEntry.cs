namespace Feedback.Api.Models;

// Named FeedbackEntry (not Feedback) because "Feedback" is already the root namespace.
public class FeedbackEntry
{
    public int Id { get; set; }                 // identity column, starts at 1
    public string Message { get; set; } = "";
    public int Rating { get; set; }
    public DateTime ReceivedAt { get; set; }    // UTC
}
