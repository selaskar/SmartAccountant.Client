namespace MAUI.MSALClient
{
    public class DownstreamApiHelper
    {
        public DownStreamApiConfig DownstreamApiConfig;

        public DownstreamApiHelper(DownStreamApiConfig downstreamApiConfig, MSALClientHelper msalClientHelper)
        {
            ArgumentNullException.ThrowIfNull(msalClientHelper);

            DownstreamApiConfig = downstreamApiConfig;
        }
    }
}
