namespace Forum.Core.Results;

public struct ValidationResult
{
    public IEnumerable<string> Errors { get; }

    public ValidationResult(IEnumerable<string> errors)
    {
        Errors = errors;
    }
}