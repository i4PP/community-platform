namespace BMWDALInterfacesAndDTOs.Exceptions;

public class NetworkException : Exception
{
    public NetworkException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
    public NetworkException(string message)
        : base(message)
    {
    }
}