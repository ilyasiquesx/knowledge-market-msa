namespace Forum.Core.Results;

public struct NotFoundResult
{
    public NotFoundResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}