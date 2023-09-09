namespace Core.Exceptions;

public class ItemNotFoundException : ApplicationException
{
    public ItemNotFoundException()
    {
    }

    public ItemNotFoundException(string message)
        : base(message)
    {
    }

    public ItemNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
