namespace SmartAccountant.ApiClient.Exceptions;

internal class CoreServiceException(string message, Exception? inner) : Exception(message, inner)
{
    public CoreServiceException(string message) : this(message, null) { }
}
