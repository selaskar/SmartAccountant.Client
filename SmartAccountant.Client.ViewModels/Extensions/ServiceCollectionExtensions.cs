using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        services.AddTransient<SignInPageModel>();

        services.AddSingleton<ICurrentUser, CurrentUser>();

        return services;
    }
}
