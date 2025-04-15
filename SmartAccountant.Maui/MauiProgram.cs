using System.Reflection;
using CommunityToolkit.Maui;
using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartAccountant.Maui.Infrastructure;
using SmartAccountant.Maui.ServiceClients;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SmartAccountant.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureMauiHandlers(handlers => { })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        builder.Services.AddSingleton<ProjectRepository>();
        builder.Services.AddSingleton<TaskRepository>();
        builder.Services.AddSingleton<CategoryRepository>();
        builder.Services.AddSingleton<TagRepository>();
        builder.Services.AddSingleton<SeedDataService>();
        builder.Services.AddSingleton<ModalErrorHandler>();
        builder.Services.AddSingleton<MainPageModel>();
        builder.Services.AddSingleton<ProjectListPageModel>();
        builder.Services.AddSingleton<ManageMetaPageModel>();

        builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
        builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");


        IConfiguration appConfiguration = GetConfig();

        AzureAdConfig? azureADConfig = appConfiguration.GetRequiredSection("AzureAd").Get<AzureAdConfig>();
        DownStreamApiConfig? downStreamApiConfig = appConfiguration.GetRequiredSection("DownstreamApi").Get<DownStreamApiConfig>();

        // configure platform specific params
        PublicClientSingleton.Instance.Initialize(azureADConfig, downStreamApiConfig);

        builder.Configuration.AddConfiguration(appConfiguration);
        builder.Services.AddOptions<CoreServiceOptions>()
            .BindConfiguration("CoreService");

        builder.Services.AddSingleton<DangerousHttpClientHandler>();

        builder.Services.AddHttpClient(nameof(CoreServiceClient))
                .ConfigurePrimaryHttpMessageHandler<DangerousHttpClientHandler>();

        builder.Services.AddScoped<ICoreServiceClient, CoreServiceClient>();

        return builder.Build();
    }

    private static IConfiguration GetConfig()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string embeddedConfigFileName = $"{assembly.GetName().Name}.appsettings.json";
        using Stream? stream = assembly.GetManifestResourceStream(embeddedConfigFileName);
        return new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
    }
}
