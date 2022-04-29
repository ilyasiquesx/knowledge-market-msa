namespace Forum.Core.Entities.Questions.Results;

public struct UserNotAllowed
{
    public UserNotAllowed(string message)
    {
        Message = message;
    }

    public string Message { get; }
}