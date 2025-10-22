using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.Dtos;

namespace SmartAccountant.ApiClient.Abstract;

public interface ICoreServiceClient
{
    /// <exception cref="CoreServiceException"/>
    /// <exception cref="OperationCanceledException"/>
    Task<IEnumerable<Account>> GetAccounts(CancellationToken cancellationToken);

    /// <exception cref="CoreServiceException"/>
    /// <exception cref="OperationCanceledException"/>
    Task<IEnumerable<Transaction>> GetTransactions(Guid accountId, CancellationToken cancellationToken);

    /// <exception cref="CoreServiceException"/>
    /// <exception cref="OperationCanceledException"/>
    Task<MonthlySummary> GetMonthlySummary(DateOnly month, CancellationToken cancellationToken);
}