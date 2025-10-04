using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.ApiClient.Infrastructure;
using SmartAccountant.ApiClient.Options;

namespace SmartAccountant.ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CoreServiceOptions>()
            .Bind(configuration);

        services.AddTransient<DangerousHttpClientHandler>();

        services.AddHttpClient(nameof(CoreServiceClient))
                .ConfigurePrimaryHttpMessageHandler<DangerousHttpClientHandler>();

        services.AddTransient<ICoreServiceClient, CoreServiceClient>();

        return services;
    }
}
