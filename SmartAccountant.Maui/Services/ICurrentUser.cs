using Microsoft.Identity.Client;

namespace SmartAccountant.Maui.Services;

public interface ICurrentUser
{
    Task<IAccount?> Account { get; }
}
