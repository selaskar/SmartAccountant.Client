using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace SmartAccountant.Client.ViewModels.Services;

internal class CurrentUser : ICurrentUser
{
    public Task<IAccount?> Account
    {
        get
        {
            return Task.Run(PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache);
        }
    }
}
