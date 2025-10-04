using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        services.AddSingleton<ICurrentUser, CurrentUser>();

        services.AddTransient<SignInPageModel>();
        services.AddTransient<AccountsPageModel>();
        services.AddTransient<TransactionsPageModel>();

        return services;
    }
}
