namespace BMWDALInterfacesAndDTOs.Exceptions;

public class DuplicateEntryException : Exception
{
    public DuplicateEntryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
    public DuplicateEntryException(string message)
        : base(message)
    {
    }
}
