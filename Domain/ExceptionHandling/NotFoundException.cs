namespace url_shortener_dotnet.Domain.ExceptionHandling;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message) : base(message)
    {
        
    }
}