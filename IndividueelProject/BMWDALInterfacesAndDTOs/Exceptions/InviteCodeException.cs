namespace BMWDALInterfacesAndDTOs.Exceptions;

public class InviteCodeException : Exception
{
    public InviteCodeException(string message) : base(message)
    {
    }

    public InviteCodeException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
}