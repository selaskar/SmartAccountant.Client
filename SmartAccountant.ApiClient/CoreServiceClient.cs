using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.ApiClient.Options;
using SmartAccountant.ApiClient.Resources;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Models;

namespace SmartAccountant.ApiClient;

internal class CoreServiceClient(IHttpClientFactory httpClientFactory, IOptions<CoreServiceOptions> options, ICurrentUser currentUser)
    : ICoreServiceClient //TODO: implement disposable
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
            throw new CoreServiceException(Messages.CannotFetchAccounts, ex);
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
            throw new CoreServiceException(Messages.CannotFetchTransactions, ex);
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
            throw new CoreServiceException(Messages.CannotFetchSummary, ex);
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

    private void SetAuthHeader(HttpClient httpClient)
    {
        string? token = currentUser.AccessToken;

        httpClient.DefaultRequestHeaders.Authorization = token != null
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }
}
