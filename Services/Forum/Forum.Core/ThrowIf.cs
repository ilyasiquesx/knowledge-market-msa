namespace Forum.Core;

public static class ThrowIf
{
    public static void NullOrEmpty(string value, string exceptionMessage)
    {
        if (string.IsNullOrEmpty(value))
            throw new DomainException(exceptionMessage);
    }

    public static void Null(object value, string exceptionMessage)
    {
        if (value == null)
            throw new DomainException(exceptionMessage);
    }

    public static void False(bool value, string exceptionMessage)
    {
        if (!value)
            throw new DomainException(exceptionMessage);
    }
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}