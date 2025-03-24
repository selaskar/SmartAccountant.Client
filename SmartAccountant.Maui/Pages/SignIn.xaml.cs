using System.Net.Http.Json;
using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace SmartAccountant.Maui.Pages;

public partial class SignIn : ContentPage
{
    public SignIn()
    {
        InitializeComponent();

        IAccount cachedUserAccount = PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache().Result;

        _ = Dispatcher.DispatchAsync(async () =>
        {
            if (cachedUserAccount == null)
                return;

            btnLogin.IsEnabled = false;
            await DisplayAlert("test", cachedUserAccount.Username, "Cancel");
        });
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string token = await PublicClientSingleton.Instance.AcquireTokenSilentAsync();


        btnLogin.IsEnabled = false;
        await DisplayAlert("test", token, "Cancel");
    }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PublicClientSingleton.Instance.SignOutAsync();

        Dispatcher.Dispatch(() => btnLogin.IsEnabled = true);
    }

    private async void btnServiceCall_Clicked(object sender, EventArgs e)
    {
        using HttpClient client = new();

        var x = PublicClientSingleton.Instance.MSALClientHelper.AuthResult.IdToken;
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", x);

        try
        {
            var openApi = await client.GetStringAsync("https://localhost:7130/openapi/v1.json");
        }
        catch (Exception ex)
        {
        }
    }
}