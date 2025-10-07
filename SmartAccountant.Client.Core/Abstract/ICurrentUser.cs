using Microsoft.Identity.Client;

namespace SmartAccountant.Client.Core.Abstract;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Task<IAccount?> Account { get; }

    string? AccessToken { get; }

    DateTimeOffset? ExpiresOn { get; }
}
