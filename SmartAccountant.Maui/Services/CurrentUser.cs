using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace SmartAccountant.Maui.Services;

public class CurrentUser : ICurrentUser
{
    public Task<IAccount?> Account
    {
        get
        {
            return Task.Run(PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache);
        }
    }
}
