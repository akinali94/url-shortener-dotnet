namespace url_shortener_dotnet.Domain.ExceptionHandling;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message) : base(message)
    {
        
    }
}