namespace url_shortener_dotnet.Domain.ExceptionHandling;

public sealed class FetchingDataException : NotFoundException
{
    public FetchingDataException(string message) : base(message)
    {
    }
}