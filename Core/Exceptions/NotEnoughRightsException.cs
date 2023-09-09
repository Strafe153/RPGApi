namespace Core.Exceptions;

public class NotEnoughRightsException : ApplicationException
{
    public NotEnoughRightsException()
    {
    }

    public NotEnoughRightsException(string message)
        : base(message)
    {
    }

    public NotEnoughRightsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
