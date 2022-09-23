namespace Core.Exceptions;

public class NameNotUniqueException : ApplicationException
{
    public NameNotUniqueException(string message)
        : base(message)
    {
    }
}
