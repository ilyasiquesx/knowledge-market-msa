namespace Forum.Core.Results;

public struct InvalidDomainBehaviorResult
{
    public InvalidDomainBehaviorResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}