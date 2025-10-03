using Microsoft.Identity.Client;

namespace SmartAccountant.Client.ViewModels.Services;

public interface ICurrentUser
{
    Task<IAccount?> Account { get; }
}
