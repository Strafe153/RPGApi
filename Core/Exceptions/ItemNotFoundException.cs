namespace Core.Exceptions;

public class ItemNotFoundException : ApplicationException
{
    public ItemNotFoundException(string message)
        : base(message)
    {
    }
}
