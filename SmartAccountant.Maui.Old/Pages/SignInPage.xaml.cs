using MAUI.MSALClient;
using Microsoft.Identity.Client;
using SmartAccountant.Maui.Old.Resources;

namespace SmartAccountant.Maui.Pages;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();

        Dispatcher.DispatchAsync(async () =>
        {
            IAccount? cachedUserAccount = await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache();

            if (cachedUserAccount == null)
                return;

            txtUserName.Text = cachedUserAccount.Username;
            await SignIn();
        });
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await SignIn();
    }

    private async Task SignIn()
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
}
