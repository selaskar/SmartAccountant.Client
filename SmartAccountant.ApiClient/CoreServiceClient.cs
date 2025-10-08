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

internal class CoreServiceClient(
    IHttpClientFactory httpClientFactory,
    IOptions<CoreServiceOptions> options,
    ICurrentUser currentUser,
    IDateTimeService dateTimeService,
    IAuthenticationService authenticationService)
    : ICoreServiceClient, IDisposable
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Account>> GetAccounts(CancellationToken cancellationToken)
    {
        try
        {
            var client = await GetHttpClient(cancellationToken);
            var accounts = await client.GetFromJsonAsync<IEnumerable<Account>>("/api/accounts", cancellationToken);

            return accounts ?? [];
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
        {
            throw new CoreServiceException(Messages.CannotFetchAccounts, ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Transaction>> GetTransactions(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var client = await GetHttpClient(cancellationToken);
            var transactions = await client.GetFromJsonAsync<IEnumerable<Transaction>>($"/api/transactions?accountId={accountId}", cancellationToken);

            return transactions ?? [];
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
        {
            throw new CoreServiceException(Messages.CannotFetchTransactions, ex);
        }
    }

    /// <inheritdoc/>
    public async Task<MonthlySummary> GetMonthlySummary(DateOnly month, CancellationToken cancellationToken)
    {
        try
        {
            var client = await GetHttpClient(cancellationToken);
            var summary = await client.GetFromJsonAsync<MonthlySummary>($"/api/summary?month={month:yyyy-MM-dd}", cancellationToken)
                ?? throw new CoreServiceException("Monthly summary was unexpectedly null.");

            return summary;
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
        {
            throw new CoreServiceException(Messages.CannotFetchSummary, ex);
        }
    }


    public void Dispose()
    {
        httpClient?.Dispose();
    }

    /// <exception cref="CoreServiceException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task<HttpClient> GetHttpClient(CancellationToken cancellationToken)
    {
        if (httpClient != null)
        {
            // in case token is refreshed since client is generated.
            await SetAuthHeader(httpClient, cancellationToken);

            return httpClient;
        }

        httpClient = httpClientFactory.CreateClient(nameof(CoreServiceClient));
        httpClient.BaseAddress = new Uri(options.Value.BaseAddress);

        await SetAuthHeader(httpClient, cancellationToken);

        return httpClient;
    }
    private HttpClient? httpClient;

    /// <exception cref="CoreServiceException"/>
    /// <exception cref="OperationCanceledException"/>
    private async Task SetAuthHeader(HttpClient httpClient, CancellationToken cancellationToken)
    {
        if (currentUser.AccessToken == null)
            throw new CoreServiceException("No active session.");

        if (currentUser.ExpiresOn!.Value.AddMinutes(-5) < dateTimeService.UtcNow)
        {
            await authenticationService.SignIn(cancellationToken);
        }

        string? token = currentUser.AccessToken;

        httpClient.DefaultRequestHeaders.Authorization = token != null
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }
}
