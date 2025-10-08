using MAUI.MSALClient;
using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.ViewModels.Services;

internal class AuthenticationService : IAuthenticationService
{
    /// <inheritdoc/>
    public async Task SignIn(CancellationToken cancellationToken)
    {
        await PublicClientSingleton.Instance.AcquireTokenSilentAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SignOut()
    {
        await PublicClientSingleton.Instance.SignOutAsync();
    }
}
