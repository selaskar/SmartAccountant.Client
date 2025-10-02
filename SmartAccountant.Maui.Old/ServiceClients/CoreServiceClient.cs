using System.Net.Http.Json;
using MAUI.MSALClient;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using SmartAccountant.Maui.Old.Resources;
using SmartAccountant.Models;

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

    /// <inheritdoc/>
    public async Task<IEnumerable<Transaction>> GetTransactions(Guid accountId)
    {
        try
        {
            var client = GetHttpClient();
            var transactions = await client.GetFromJsonAsync<IEnumerable<Transaction>>($"/api/transactions?accountId={accountId}");

            return transactions ?? [];
        }
        catch (Exception ex)
        {
            throw new CoreServiceException(Message.CannotFetchTransactions, ex);
        }
    }

    /// <inheritdoc/>
    public async Task<MonthlySummary> GetMonthlySummary(DateOnly month)
    {
        try
        {
            var client = GetHttpClient();
            var summary = await client.GetFromJsonAsync<MonthlySummary>($"/api/summary?month={month:yyyy-MM-dd}");

            return summary;
        }
        catch (Exception ex)
        {
            throw new CoreServiceException(Message.CannotFetchSummary, ex);
        }
    }

    private HttpClient GetHttpClient()
    {
        if (httpClient != null)
        {
            // in case token is refreshed since client is generated.
            SetAuthHeader(httpClient);
            return httpClient;
        }

        httpClient = httpClientFactory.CreateClient(nameof(CoreServiceClient));
        httpClient.BaseAddress = new Uri(options.Value.BaseAddress);

        SetAuthHeader(httpClient);

        return httpClient;
    }
    private HttpClient? httpClient;

    private static void SetAuthHeader(HttpClient httpClient)
    {
        string? token = PublicClientSingleton.Instance.MSALClientHelper.AuthResult?.AccessToken;

        httpClient.DefaultRequestHeaders.Authorization = token != null ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }
}
