using System.Reflection;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace SmartAccountant.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        IConfiguration appConfiguration = GetConfig();
        AzureAdConfig? azureADConfig = appConfiguration.GetRequiredSection("AzureAd").Get<AzureAdConfig>();
        DownStreamApiConfig? downStreamApiConfig = appConfiguration.GetRequiredSection("DownstreamApi").Get<DownStreamApiConfig>();

        // configure platform specific params
        PublicClientSingleton.Instance.Initialize(azureADConfig, downStreamApiConfig);
        PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";
        PlatformConfig.Instance.ParentWindow = this;

        // Initialize MSAL and platformConfig is set
        _ = Task.Run(PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync).Result;
    }

    private IConfiguration GetConfig()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string embeddedConfigfilename = $"{assembly.GetName().Name}.appsettings.json";
        using var stream = assembly.GetManifestResourceStream(embeddedConfigfilename);
        return new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
}
