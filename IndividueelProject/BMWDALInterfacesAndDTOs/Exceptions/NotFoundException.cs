namespace BMWDALInterfacesAndDTOs.Exceptions;


public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base()
    {
    }

    public NotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}