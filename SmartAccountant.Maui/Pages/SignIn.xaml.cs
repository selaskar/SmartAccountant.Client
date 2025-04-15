using MAUI.MSALClient;
using Microsoft.Identity.Client;
using SmartAccountant.Maui.Resources;
using SmartAccountant.Maui.ServiceClients;

namespace SmartAccountant.Maui.Pages;

public partial class SignIn : ContentPage
{
    private readonly ICoreServiceClient serviceClient;

    public SignIn(ICoreServiceClient serviceClient)
    {
        InitializeComponent();

        IAccount? cachedUserAccount = PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache().Result;

        Dispatcher.Dispatch(() =>
        {
            if (cachedUserAccount == null)
                return;

            txtUserName.Text = cachedUserAccount.Username;
        });

        this.serviceClient = serviceClient;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
            IAccount? cachedUserAccount = await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache();
            txtUserName.Text = cachedUserAccount?.Username;
        }
        catch (Exception ex)
        {
            await DisplayAlert(Message.Error, ex.ToString(), Message.OK);
        }
        finally
        {
            btnLogin.IsEnabled = false;
        }
    }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PublicClientSingleton.Instance.SignOutAsync();

        Dispatcher.Dispatch(() =>
        {
            txtUserName.Text = "";
            btnLogin.IsEnabled = true;
        });
    }

    private async void btnServiceCall_Clicked(object sender, EventArgs e)
    {
        if (PublicClientSingleton.Instance.MSALClientHelper.AuthResult == null)
        {
            await DisplayAlert(Message.Error, Message.UserNotAuthenticated, Message.Cancel);
            return;
        }

        try
        {
            IEnumerable<Account> accounts = await serviceClient.GetAccounts();
            await DisplayAlert("Accounts", string.Join(" - ", accounts.Select(x => x.FriendlyName)), Message.Cancel);
        }
        catch (CoreServiceException ex)
        {
            await DisplayAlert(Message.Error, ex.Message, Message.Cancel);
        }
    }
}
