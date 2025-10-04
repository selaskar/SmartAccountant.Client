using MAUI.MSALClient;
using Microsoft.Identity.Client;
using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.ViewModels.Services;

internal class CurrentUser : ICurrentUser
{
    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public Task<IAccount?> Account
    {
        get
        {
            return Task.Run(PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache);
        }
    }

    public string? AccessToken => PublicClientSingleton.Instance.MSALClientHelper.AuthResult?.AccessToken;
}
