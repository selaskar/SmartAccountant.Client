using System.Net.Security;

namespace SmartAccountant.ApiClient.Infrastructure;

internal partial class DangerousHttpClientHandler : HttpClientHandler
{
    /// <summary>
    /// Dismisses certificate-related errors of HTTPS in debug mode.
    /// </summary>
    public DangerousHttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
#if !DEBUG
            return false;
#endif

            return sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == (SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors);
        };
    }
}