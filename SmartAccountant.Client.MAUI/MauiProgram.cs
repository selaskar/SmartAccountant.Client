using System.Reflection;
using CommunityToolkit.Maui;
using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartAccountant.Client.MAUI.Services;
using SmartAccountant.Client.ViewModels.Extensions;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        IConfiguration appConfiguration = GetConfig();
        builder.Configuration.AddConfiguration(appConfiguration);

        ConfigureAuthentication(appConfiguration);

        builder.Services.RegisterViewModels();

        builder.Services.AddSingleton<IErrorHandler, ModalErrorHandler>();

        return builder.Build();
    }

    private static void ConfigureAuthentication(IConfiguration appConfiguration)
    {
        AzureAdConfig? azureADConfig = appConfiguration.GetRequiredSection("AzureAd").Get<AzureAdConfig>();
        DownStreamApiConfig? downStreamApiConfig = appConfiguration.GetRequiredSection("DownstreamApi").Get<DownStreamApiConfig>();

        if (azureADConfig == null || downStreamApiConfig == null)
            throw new Exception("Entra ID or downstream API configuration is missing.");

        PublicClientSingleton.Instance.Initialize(azureADConfig, downStreamApiConfig);
    }


    private static IConfiguration GetConfig()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string embeddedConfigFileName = $"{assembly.GetName().Name}.appsettings.json";
        using Stream? stream = assembly.GetManifestResourceStream(embeddedConfigFileName);

        if (stream == null)
            throw new Exception("app.settings file could not be found");

        return new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
    }
}
