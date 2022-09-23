namespace Core.Exceptions;

public class NotEnoughRightsException : ApplicationException
{
    public NotEnoughRightsException(string message)
        : base(message)
    {
    }
}
