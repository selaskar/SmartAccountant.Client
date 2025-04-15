using System.Net.Http.Json;
using MAUI.MSALClient;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using SmartAccountant.Maui.Resources;

namespace SmartAccountant.Maui.ServiceClients;

public class CoreServiceClient(IHttpClientFactory httpClientFactory, IOptions<CoreServiceOptions> options) : ICoreServiceClient //TODO: implement disposable
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Account>> GetAccounts()
    {
        try
        {
            var client = GetHttpClient();
            var accounts = await client.GetFromJsonAsync<IEnumerable<Account>>("/api/accounts");

            return accounts ?? [];
        }
        catch (Exception ex)
        {
            throw new CoreServiceException(Message.CannotFetchAccounts, ex);
        }
    }

    private HttpClient GetHttpClient()
    {
        if (_httpClient != null)
            return _httpClient;

        _httpClient = httpClientFactory.CreateClient(nameof(CoreServiceClient));
        _httpClient.BaseAddress = new Uri(options.Value.BaseAddress);

        string? token = PublicClientSingleton.Instance.MSALClientHelper.AuthResult?.AccessToken;

        if (token != null)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return _httpClient;
    }
    private HttpClient? _httpClient;
}
