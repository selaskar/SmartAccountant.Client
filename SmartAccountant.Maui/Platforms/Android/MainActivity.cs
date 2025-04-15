using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace SmartAccountant.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        //TODO: dynamic code may be replaced with hardcoded URI to relieve from static singleton instance.
        PlatformConfig.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";
        PlatformConfig.ParentWindow = this;
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
}
