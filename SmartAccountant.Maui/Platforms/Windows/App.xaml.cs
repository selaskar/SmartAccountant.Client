using System.Reflection;
using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartAccountant.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		InitializeComponent();

        IConfiguration appConfiguration = GetConfig();
        AzureAdConfig? azureAdConfig = appConfiguration.GetRequiredSection("AzureAd").Get<AzureAdConfig>();
        DownStreamApiConfig? downStreamApiConfig = appConfiguration.GetRequiredSection("DownstreamApi").Get<DownStreamApiConfig>();

        PublicClientSingleton.Instance.Initialize(azureAdConfig, downStreamApiConfig);

        // configure redirect URI for your application
        PlatformConfig.Instance.RedirectUri = "http://localhost";// $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";

        // Initialize MSAL
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

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();


    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var app = SmartAccountant.Maui.App.Current;
        PlatformConfig.Instance.ParentWindow = ((MauiWinUIWindow)app.Windows[0].Handler.PlatformView).WindowHandle;
    }
}

