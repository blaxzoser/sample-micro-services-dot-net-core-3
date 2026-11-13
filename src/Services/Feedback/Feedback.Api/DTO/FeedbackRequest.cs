namespace Feedback.Api.DTO;

public class FeedbackRequest
{
    // TODO (POST-1): message is required, not empty, not only spaces, max 500 characters.
    public string Message { get; set; } = "";

    // TODO (POST-1): rating must be between 1 and 5.
    public int Rating { get; set; }
}
