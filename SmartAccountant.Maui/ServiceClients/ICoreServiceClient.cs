namespace SmartAccountant.Maui.ServiceClients;

public interface ICoreServiceClient
{
    /// <exception cref="CoreServiceException"/>
    Task<IEnumerable<Account>> GetAccounts();
}