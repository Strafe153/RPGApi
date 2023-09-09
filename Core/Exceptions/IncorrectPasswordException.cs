namespace Core.Exceptions;

public class IncorrectPasswordException : ApplicationException
{
    public IncorrectPasswordException()
    {
    }

    public IncorrectPasswordException(string message) 
        : base(message)
    {
    }

    public IncorrectPasswordException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
