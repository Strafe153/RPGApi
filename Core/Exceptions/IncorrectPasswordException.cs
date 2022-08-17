namespace Core.Exceptions
{
    public class IncorrectPasswordException : ApplicationException
    {
        public IncorrectPasswordException(string message) 
            : base(message)
        {
        }
    }
}
