using Microsoft.Identity.Client;

namespace SmartAccountant.Client.Core.Extensions;

public static class IAccountExtensions
{
    public static string? GetDisplayName(this IAccount account)
    {
        TenantProfile? tenantProfile = account.GetTenantProfiles().FirstOrDefault();
        return tenantProfile?.ClaimsPrincipal.FindFirst("name")?.Value;
    }
}
