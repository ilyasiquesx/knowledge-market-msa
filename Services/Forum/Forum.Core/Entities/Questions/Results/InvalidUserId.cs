namespace Forum.Core.Entities.Questions.Results;

public struct InvalidUserId
{
    public InvalidUserId(string message)
    {
        Message = message;
    }

    public string Message { get; }
}