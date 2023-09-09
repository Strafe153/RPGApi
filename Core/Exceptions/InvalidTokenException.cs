namespace Core.Exceptions;

public class InvalidTokenException : ApplicationException
{
    public InvalidTokenException()
    {
    }

    public InvalidTokenException(string message)
		: base(message)
	{
	}

    public InvalidTokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
