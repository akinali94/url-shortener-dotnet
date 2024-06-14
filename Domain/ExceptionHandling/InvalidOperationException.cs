namespace url_shortener_dotnet.Domain.ExceptionHandling;

public sealed class InvalidOperationException : BadRequestException
{
    public InvalidOperationException(string message) : base(message)
    {
    }
}