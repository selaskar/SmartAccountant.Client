
namespace SmartAccountant.Maui.ServiceClients;

internal class CoreServiceException(string message, Exception? inner) : Exception(message, inner)
{
}
