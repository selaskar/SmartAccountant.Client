using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        services.AddSingleton<ICurrentUser, CurrentUser>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        services.AddTransient<AccountsPageModel>();
        services.AddTransient<SignInPageModel>();
        services.AddTransient<SummaryPageModel>();
        services.AddTransient<TransactionsPageModel>();
        services.AddTransient<DebitTransactionDetailsPageModel>();
        services.AddTransient<CreditCardTransactionDetailsPageModel>();

        return services;
    }
}
