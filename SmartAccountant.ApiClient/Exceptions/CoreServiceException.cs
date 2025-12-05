namespace SmartAccountant.ApiClient.Exceptions;

public class CoreServiceException(string message, Exception? inner) : Exception(message, inner)
{
    public CoreServiceException(string message) : this(message, null) { }
}
