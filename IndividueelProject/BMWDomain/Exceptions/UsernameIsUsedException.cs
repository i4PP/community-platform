namespace BMWDomain.Exceptions;

public class UsernameIsUsedException : Exception
{
    public UsernameIsUsedException(string message) : base(message)
    {
    }
    
    public UsernameIsUsedException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
}