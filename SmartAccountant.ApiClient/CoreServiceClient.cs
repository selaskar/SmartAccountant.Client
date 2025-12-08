using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.ApiClient.Options;
using SmartAccountant.ApiClient.Resources;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Models;

namespace SmartAccountant.ApiClient;

//TODO: In Android, instead of OperationCanceledException, socket closed error is thrown,
//when cancelling HttpClient requests.
//Handle them separately and accurately (shouldn't handle similar, unrelated errors).
internal partial class CoreServiceClient(
    IHttpClientFactory httpClientFactory,
    IOptions<CoreServiceOptions> options,
    ICurrentUser currentUser,
    IDateTimeService dateTimeService,
    IAuthenticationService authenticationService,
    IMapper mapper,
    ILogger<CoreServiceClient> logger)
    : ICoreServiceClient, IDisposable
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Account>> GetAccounts(CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = await GetHttpClient(cancellationToken);
            var accounts = await client.GetFromJsonAsync<IEnumerable<Dtos.Account>>("/api/accounts", cancellationToken);

            return (accounts ?? []).Select(mapper.Map<Dtos.Account, Account>);
        }
        catch (CoreServiceException ex)
        {
            CoreServiceExceptionOccurred(ex);
            throw;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var coreServiceException = new CoreServiceException(Messages.CannotFetchAccounts, ex);
            CoreServiceExceptionOccurred(coreServiceException);
            throw coreServiceException;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Transaction>> GetTransactions(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = await GetHttpClient(cancellationToken);
            var transactions = await client.GetFromJsonAsync<IEnumerable<Dtos.Transaction>>($"/api/transactions?accountId={accountId}", cancellationToken);

            return (transactions ?? []).Select(mapper.Map<Dtos.Transaction, Transaction>);
        }
        catch (CoreServiceException ex)
        {
            CoreServiceExceptionOccurred(ex);
            throw;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var coreServiceException = new CoreServiceException(Messages.CannotFetchTransactions, ex);
            CoreServiceExceptionOccurred(coreServiceException);
            throw coreServiceException;
        }
    }

    /// <inheritdoc/>
    public async Task<MonthlySummary> GetMonthlySummary(DateOnly month, CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = await GetHttpClient(cancellationToken);
            var summary = await client.GetFromJsonAsync<Dtos.MonthlySummary>($"/api/summary?month={month:yyyy-MM-dd}", cancellationToken)
                ?? throw new CoreServiceException(Messages.EmptyMonthlySummaryResponse);

            return mapper.Map<Dtos.MonthlySummary, MonthlySummary>(summary);
        }
        catch (CoreServiceException ex)
        {
            CoreServiceExceptionOccurred(ex);
            throw;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var coreServiceException = new CoreServiceException(Messages.CannotFetchSummary, ex);
            CoreServiceExceptionOccurred(coreServiceException);
            throw coreServiceException;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateDebitTransactionAsync(DebitTransaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = await GetHttpClient(cancellationToken);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync("/api/transactions/debit", transaction, cancellationToken);
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (CoreServiceException ex)
        {
            CoreServiceExceptionOccurred(ex);
            throw;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var coreServiceException = new CoreServiceException(Messages.CannotUpdateDebitTransaction, ex);
            CoreServiceExceptionOccurred(coreServiceException);
            throw coreServiceException;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateCreditCardTransactionAsync(CreditCardTransaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = await GetHttpClient(cancellationToken);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync("/api/transactions/cc", transaction, cancellationToken);
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (CoreServiceException ex)
        {
            CoreServiceExceptionOccurred(ex);
            throw;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var coreServiceException = new CoreServiceException(Messages.CannotUpdateCreditCardTransaction, ex);
            CoreServiceExceptionOccurred(coreServiceException);
            throw coreServiceException;
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
            throw new CoreServiceException(Messages.UserNotAuthenticated);

        if (currentUser.ExpiresOn!.Value.AddMinutes(-5) < dateTimeService.UtcNow)
            await authenticationService.SignIn(cancellationToken);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);
    }


    [LoggerMessage(LogLevel.Error)]
    private protected partial void CoreServiceExceptionOccurred(Exception ex);
}
