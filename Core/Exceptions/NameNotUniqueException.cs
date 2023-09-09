namespace Core.Exceptions;

public class NameNotUniqueException : ApplicationException
{
    public NameNotUniqueException()
    {
    }

    public NameNotUniqueException(string message)
        : base(message)
    {
    }

    public NameNotUniqueException(string message, Exception innerException)
        : base (message, innerException)
    {
    }
}
