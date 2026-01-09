using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.ApiClient.Options;
using SmartAccountant.ApiClient.Resources;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Models;
using SmartAccountant.Dtos.Response;
using SmartAccountant.Shared.Enums.Errors;

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

            HttpResponseMessage responseMessage = await client.GetAsync(new Uri("/api/accounts", UriKind.Relative), cancellationToken);

            return await ParseCollectionResponse<Dtos.Account, Account, AccountErrors>(responseMessage, Messages.CannotFetchAccounts, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
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

            HttpResponseMessage responseMessage = await client.GetAsync(new Uri($"/api/transactions?accountId={accountId}", UriKind.Relative), cancellationToken);

            return await ParseCollectionResponse<Dtos.Transaction, Transaction, TransactionErrors>(responseMessage, Messages.CannotFetchTransactions, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
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

            HttpResponseMessage responseMessage = await client.GetAsync(new Uri($"/api/summary?month={month:yyyy-MM-dd}", UriKind.Relative), cancellationToken)
                ?? throw new CoreServiceException(Messages.EmptyMonthlySummaryResponse);

            return await ParseSingleResponse<Dtos.MonthlySummary, MonthlySummary, SummaryErrors>(responseMessage, Messages.CannotFetchSummary, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
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

            if (!responseMessage.IsSuccessStatusCode)
                await ErrorHandleCommon<object, TransactionErrors>(responseMessage, Messages.CannotUpdateDebitTransaction, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
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

            if (!responseMessage.IsSuccessStatusCode)
                await ErrorHandleCommon<object, TransactionErrors>(responseMessage, Messages.CannotUpdateCreditCardTransaction, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not CoreServiceException)
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
            throw new CoreServiceException(Messages.ReauthenticationRequired);

        if (currentUser.ExpiresOn!.Value.AddMinutes(-5) < dateTimeService.UtcNow)
            await authenticationService.SignIn(cancellationToken);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);
    }


    /// <exception cref="CoreServiceException"></exception>
    private async Task<TModel> ParseSingleResponse<TDto, TModel, TErrors>(HttpResponseMessage responseMessage, string unspecifiedErrorMessage, CancellationToken cancellationToken)
        where TErrors : struct, Enum
    {
        if (responseMessage.IsSuccessStatusCode)
        {
            var dto = await responseMessage.Content.ReadFromJsonAsync<TDto>(cancellationToken);
            return mapper.Map<TDto, TModel>(dto);
        }

        return await ErrorHandleCommon<TModel, TErrors>(responseMessage, unspecifiedErrorMessage, cancellationToken);
    }

    /// <exception cref="CoreServiceException"></exception>
    private async Task<IEnumerable<TModel>> ParseCollectionResponse<TDto, TModel, TErrors>(HttpResponseMessage responseMessage, string unspecifiedErrorMessage, CancellationToken cancellationToken)
        where TErrors : struct, Enum
    {
        if (responseMessage.IsSuccessStatusCode)
        {
            var dtos = await responseMessage.Content.ReadFromJsonAsync<IEnumerable<TDto>>(cancellationToken);
            return (dtos ?? []).Select(mapper.Map<TDto, TModel>);
        }

        return await ErrorHandleCommon<IEnumerable<TModel>, TErrors>(responseMessage, unspecifiedErrorMessage, cancellationToken);
    }

    /// <exception cref="CoreServiceException"></exception>
    private async Task<TReturn> ErrorHandleCommon<TReturn, TErrors>(HttpResponseMessage responseMessage, string unspecifiedErrorMessage, CancellationToken cancellationToken)
        where TErrors : struct, Enum
    {
        // Error-handling middleware on server-side wraps unhandled exceptions into 500 status code.
        // Anything with greater code is abnormal.
        if ((int)responseMessage.StatusCode > 500)
        {
            CoreServiceExceptionOccurred(responseMessage.StatusCode.ToString());
            throw new CoreServiceException(unspecifiedErrorMessage);
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            throw new CoreServiceException(Messages.ReauthenticationRequired);

        if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
            throw new CoreServiceException(Messages.Unauthorized);

        ErrorDetail errorDetail = await responseMessage.Content.ReadFromJsonAsync<ErrorDetail>(cancellationToken)
            ?? throw new CoreServiceException(unspecifiedErrorMessage);

        if (errorDetail.Category == ErrorCategory.EnumException)
        {
            string enumMember = Enum.GetName(typeof(TErrors), errorDetail.Code)
                ?? default(TErrors).ToString();

            string? localError = typeof(TErrors).GetField(enumMember)
                ?.GetCustomAttribute<DisplayAttribute>()
                ?.GetName();

            throw new CoreServiceException(localError ?? enumMember);
        }
        else if (errorDetail.Category == ErrorCategory.ValidationException)
            throw new CoreServiceException(errorDetail.Detail!);
        else
        {
            CoreServiceExceptionOccurred(errorDetail.Detail!);
            throw new CoreServiceException(unspecifiedErrorMessage);
        }
    }

    [LoggerMessage(LogLevel.Error)]
    private protected partial void CoreServiceExceptionOccurred(Exception ex);

    [LoggerMessage(LogLevel.Error, Message = "An unexpected server error occurred ({Detail}).")]
    private protected partial void CoreServiceExceptionOccurred(string detail);
}
