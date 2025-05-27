namespace BMWDomain.Exceptions;

public class BllException : Exception
{
    public BllException(string message) : base(message)
    {
    }

    public BllException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
}