using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.ApiClient.Abstract;

public interface ICoreServiceClient
{
    /// <exception cref="CoreServiceException"/>
    Task<IEnumerable<Account>> GetAccounts();

    /// <exception cref="CoreServiceException"/>
    Task<IEnumerable<Transaction>> GetTransactions(Guid accountId);

    /// <exception cref="CoreServiceException"/>
    Task<MonthlySummary> GetMonthlySummary(DateOnly month);
}