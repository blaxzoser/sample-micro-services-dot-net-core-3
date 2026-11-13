namespace Feedback.Api.DTO;

public record FeedbackResponse(int Id, string Message, int Rating, DateTime ReceivedAt);
