using System.Reflection;
using Azure.Monitor.OpenTelemetry.Exporter;
using CommunityToolkit.Maui;
using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartAccountant.ApiClient.Extensions;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.MAUI.Pages;
using SmartAccountant.Client.MAUI.Services;
using SmartAccountant.Client.ViewModels.Extensions;
using SmartAccountant.Client.ViewModels.Services;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SmartAccountant.Client.MAUI;

internal static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldEnableSnackbarOnWindows(true);
            })
            .ConfigureSyncfusionCore()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        IConfiguration appConfiguration = GetConfig();
        builder.Configuration.AddConfiguration(appConfiguration);

        ConfigureLogging(builder.Services, builder.Logging, appConfiguration);

        SyncfusionLicenseProvider.RegisterLicense(appConfiguration["Syncfusion:LicenseKey"]);

        ConfigureAuthentication(appConfiguration);

        builder.Services.AddSingleton<IErrorHandler, ModalErrorHandler>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.RegisterViewModels();

        builder.Services.RegisterApiClient(appConfiguration.GetRequiredSection("CoreService"));

        RegisterRoutes();

        return builder.Build();
    }

    private static void ConfigureLogging(IServiceCollection services, ILoggingBuilder loggingBuilder, IConfiguration appConfiguration)
    {
#if DEBUG
        loggingBuilder.AddDebug();
#endif

        //OpenTelemetryBuilder.UseAzureMonitorExporter or ILoggingBuilder.AddOpenTelemetry don't work for some reason.
        //This is the only working variation.
        services.AddOpenTelemetry()
            .WithLogging(x => x.AddAzureMonitorLogExporter(o => o.ConnectionString = appConfiguration["AzureMonitor:ConnectionString"]));        
    }

    private static void ConfigureAuthentication(IConfiguration appConfiguration)
    {
        AzureAdConfig? azureADConfig = appConfiguration.GetRequiredSection("AzureAd").Get<AzureAdConfig>();
        DownStreamApiConfig? downStreamApiConfig = appConfiguration.GetRequiredSection("DownstreamApi").Get<DownStreamApiConfig>();

        if (azureADConfig == null || downStreamApiConfig == null)
            throw new Exception("Entra ID or downstream API configuration is missing.");

        PublicClientSingleton.Instance.Initialize(azureADConfig, downStreamApiConfig);
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("//accounts/transactions", typeof(TransactionsPage));
        Routing.RegisterRoute("//accounts/transactions/details-debit", typeof(DebitTransactionDetailsPage));
        Routing.RegisterRoute("//accounts/transactions/details-creditCard", typeof(CreditCardTransactionDetailsPage));
    }

    private static IConfiguration GetConfig()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string embeddedConfigFileName = $"{assembly.GetName().Name}.appsettings.json";
        using Stream? stream = assembly.GetManifestResourceStream(embeddedConfigFileName)
            ?? throw new Exception("app.settings file could not be found");

        return new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
    }
}
