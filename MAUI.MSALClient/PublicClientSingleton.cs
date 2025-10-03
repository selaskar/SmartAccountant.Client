// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Microsoft.Identity.Client;

namespace MAUI.MSALClient
{
    /// <summary>
    /// This is a singleton implementation to wrap the MSALClient and associated classes to support static initialization model for platforms that need this.
    /// </summary>
    public class PublicClientSingleton
    {
        /// <summary>
        /// This is the singleton used by Ux. Since PublicClientWrapper constructor does not have perf or memory issue, it is instantiated directly.
        /// </summary>
        public static PublicClientSingleton Instance { get; private set; } = new PublicClientSingleton();

        /// <summary>
        /// Gets the instance of MSALClientHelper.
        /// </summary>
        public DownstreamApiHelper DownstreamApiHelper { get; private set; } = null!;

        /// <summary>
        /// Gets the instance of MSALClientHelper.
        /// </summary>
        public MSALClientHelper MSALClientHelper { get; private set; } = null!;

        /// <summary>
        /// This will determine if the Interactive Authentication should be Embedded or System view
        /// </summary>
        public bool UseEmbedded { get; set; } = false;

        /// <summary>
        /// Prevents a default instance of the <see cref="PublicClientSingleton"/> class from being created. or a private constructor for singleton
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private PublicClientSingleton()
        {
        }

        public void Initialize(AzureAdConfig azureADConfig, DownStreamApiConfig downStreamApiConfig)
        {
            MSALClientHelper = new MSALClientHelper(azureADConfig);

            DownstreamApiHelper = new DownstreamApiHelper(downStreamApiConfig, MSALClientHelper);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync(CancellationToken cancellationToken)
        {
            // Get accounts by policy
            return await this.AcquireTokenSilentAsync(this.GetScopes(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync(string[] scopes, CancellationToken cancellationToken)
        {
            return await this.MSALClientHelper.SignInUserAndAcquireAccessToken(scopes, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Perform the interactive acquisition of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns></returns>
        internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes, CancellationToken cancellationToken)
        {
            this.MSALClientHelper.UseEmbedded = this.UseEmbedded;
            return await this.MSALClientHelper.SignInUserInteractivelyAsync(scopes, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// It will sign out the user.
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            await this.MSALClientHelper.SignOutUserAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets scopes for the application
        /// </summary>
        /// <returns>An array of all scopes</returns>
        internal string[] GetScopes()
        {
            return this.DownstreamApiHelper.DownstreamApiConfig.ScopesArray;
        }
    }
}