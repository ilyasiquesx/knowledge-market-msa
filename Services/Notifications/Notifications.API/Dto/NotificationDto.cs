namespace Notifications.API.Dto;

public class NotificationDto
{
    public string Message { get; set; }
    public long QuestionId { get; set; }
    public string QuestionTitle { get; set; }
}