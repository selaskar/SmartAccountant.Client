﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Abstractions;


#if WINDOWS
using Microsoft.Identity.Client.Desktop;
#endif

namespace MAUI.MSALClient
{
    /// <summary>
    /// Contains methods that initialize and use the MSAL SDK
    /// </summary>
    public class MSALClientHelper
    {
        /// <summary>
        /// As for the Tenant, you can use a name as obtained from the azure portal, e.g. kko365.onmicrosoft.com"
        /// </summary>
        public AzureAdConfig AzureAdConfig;

        /// <summary>
        /// Gets the authentication result (if available) from MSAL's various operations.
        /// </summary>
        /// <value>
        /// The authentication result.
        /// </value>
        public AuthenticationResult? AuthResult { get; private set; }

        /// <summary>
        /// Gets the MSAL public client application instance.
        /// </summary>
        /// <value>
        /// The public client application.
        /// </value>
        public IPublicClientApplication? PublicClientApplication { get; private set; }

        /// <summary>
        /// This will determine if the Interactive Authentication should be Embedded or System view
        /// </summary>
        public bool UseEmbedded { get; set; } = false;

        /// <summary>
        /// The PublicClientApplication builder used internally
        /// </summary>
        private PublicClientApplicationBuilder PublicClientApplicationBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MSALClientHelper"/> class.
        /// </summary>
        public MSALClientHelper(AzureAdConfig azureAdConfig)
        {
            AzureAdConfig = azureAdConfig;

            this.InitializePublicClientApplicationBuilder();
        }

        /// <summary>
        /// Initializes the MSAL's PublicClientApplication builder from config.
        /// </summary>
        /// <autogeneratedoc />
        [MemberNotNull(nameof(PublicClientApplicationBuilder))]
        private void InitializePublicClientApplicationBuilder()
        {
            this.PublicClientApplicationBuilder = PublicClientApplicationBuilder.Create(AzureAdConfig.ClientId)
                .WithAuthority(AzureAdConfig.Authority)
                .WithExperimentalFeatures() // this is for upcoming logger
                .WithLogging(new IdentityLogger(EventLogLevel.Warning), enablePiiLogging: false)    // This is the currently recommended way to log MSAL message. For more info refer to https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/logging. Set Identity Logging level to Warning which is a middle ground
                .WithClientCapabilities(["cp1"])                                     // declare this client app capable of receiving CAE events- https://aka.ms/clientcae
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache");
        }

        /// <summary>
        /// Initializes the public client application of MSAL.NET with the required information to correctly authenticate the user.
        /// </summary>
        /// <returns></returns>
        public async Task InitializePublicClientAppAsync()
        {
            if (PlatformConfig.RedirectUri == null)
            {
                await PlatformConfig.WaitForRedirectUriAsync();
            }

            // Initialize the MSAL library by building a public client application
            this.PublicClientApplication = this.PublicClientApplicationBuilder
                .WithRedirectUri(PlatformConfig.RedirectUri)   // redirect URI is set later in PlatformConfig when the platform has been decided
#if WINDOWS
                .WithWindowsEmbeddedBrowserSupport()
#endif
                .Build();

            await AttachTokenCache();
        }

        /// <summary>
        /// Attaches the token cache to the Public Client app.
        /// </summary>
        /// <returns>IAccount list of already signed-in users (if available)</returns>
        private async Task<IEnumerable<IAccount>> AttachTokenCache()
        {
            if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
            {
                return null;
            }

            // Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
            var storageProperties = new StorageCreationPropertiesBuilder(AzureAdConfig.CacheFileName, AzureAdConfig.CacheDir)
                    .Build();

            var msalCacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
            msalCacheHelper.RegisterCache(PublicClientApplication.UserTokenCache);

            // If the cache file is being reused, we'd find some already-signed-in accounts
            return await PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Signs in the user and obtains an Access token for a provided set of scopes
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns> Access Token</returns>
        public async Task<string> SignInUserAndAcquireAccessToken(string[] scopes)
        {
            if (this.PublicClientApplication == null)
                await InitializePublicClientAppAsync();

            var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

            try
            {
                // 1. Try to sign-in the previously signed-in account
                if (existingUser != null)
                {
                    this.AuthResult = await this.PublicClientApplication
                        .AcquireTokenSilent(scopes, existingUser)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    this.AuthResult = await SignInUserInteractivelyAsync(scopes);
                }
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenInteractive to acquire a token interactively
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                //this.AuthResult = await this.PublicClientApplication
                //    .AcquireTokenInteractive(scopes)
                //    .WithLoginHint(existingUser?.Username ?? string.Empty)
                //    .ExecuteAsync()
                //    .ConfigureAwait(false);
                throw;
            }
            catch (MsalException msalEx)
            {
                Debug.WriteLine($"Error Acquiring Token interactively:{Environment.NewLine}{msalEx}");
                throw;
            }

            return this.AuthResult.AccessToken;
        }

        /// <summary>
        /// Signs the in user and acquire access token for a provided set of scopes.
        /// </summary>
        /// <param name="scopes">The scopes.</param>
        /// <param name="extraclaims">The extra claims, usually from CAE. We basically handle CAE by sending the user back to Azure AD for
        /// additional processing and requesting a new access token for Graph</param>
        /// <returns></returns>
        public async Task<string> SignInUserAndAcquireAccessToken(string[] scopes, string extraclaims)
        {
            if (this.PublicClientApplication == null)
                await InitializePublicClientAppAsync();

            try
            {
                // Send the user to Azure AD for re-authentication as a silent acquisition wont resolve any CAE scenarios like an extra claims request
                this.AuthResult = await this.PublicClientApplication.AcquireTokenInteractive(scopes)
                        .WithClaims(extraclaims)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
            }
            catch (MsalException msalEx)
            {
                Debug.WriteLine($"Error Acquiring Token:{Environment.NewLine}{msalEx}");
            }

            return this.AuthResult.AccessToken;
        }

        /// <summary>
        /// Shows a pattern to sign-in a user interactively in applications that are input constrained and would need to fall-back on device code flow.
        /// </summary>
        /// <param name="scopes">The scopes.</param>
        /// <param name="existingAccount">The existing account.</param>
        /// <returns></returns>
        public async Task<AuthenticationResult> SignInUserInteractivelyAsync(string[] scopes, IAccount existingAccount = null)
        {
            if (this.PublicClientApplication == null)
                await InitializePublicClientAppAsync();

            // If the operating system has UI
            if (this.PublicClientApplication.IsUserInteractive())
            {
                SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();
#if IOS
                // Hide the privacy prompt in iOS
                systemWebViewOptions.iOSHidePrivacyPrompt = true;
#endif
                return await this.PublicClientApplication.AcquireTokenInteractive(scopes)
                    .WithLoginHint(existingAccount?.Username ?? String.Empty)
                    .WithSystemWebViewOptions(systemWebViewOptions)
                    .WithParentActivityOrWindow(PlatformConfig.ParentWindow)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }

            // If the operating system does not have UI (e.g. SSH into Linux), you can fallback to device code, however this
            // flow will not satisfy the "device is managed" CA policy.
            return await this.PublicClientApplication.AcquireTokenWithDeviceCode(scopes, (dcr) =>
            {
                Console.WriteLine(dcr.Message);
                return Task.CompletedTask;
            }).ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the first signed-in user's record from token cache
        /// </summary>
        public async Task SignOutUserAsync()
        {
            IAccount? existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

            if (existingUser == null)
                return;

            await this.SignOutUserAsync(existingUser).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a given user's record from token cache
        /// </summary>
        /// <param name="user">The user.</param>
        public async Task SignOutUserAsync(IAccount user)
        {
            if (this.PublicClientApplication == null) return;

            await this.PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches the signed in user from MSAL's token cache (if available).
        /// </summary>
        /// <returns></returns>
        public async Task<IAccount?> FetchSignedInUserFromCache()
        {
            if (this.PublicClientApplication == null)
                await InitializePublicClientAppAsync();

            // get accounts from cache
            IEnumerable<IAccount> accounts = await this.PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);

            // Error corner case: we should always have 0 or 1 accounts, not expecting > 1
            // This is just an example of how to resolve this ambiguity, which can arise if more apps share a token cache.
            // Note that some apps prefer to use a random account from the cache.
            if (accounts.Count() > 1)
            {
                foreach (var acc in accounts)
                {
                    await this.PublicClientApplication.RemoveAsync(acc);
                }

                return null;
            }

            return accounts.SingleOrDefault();
        }
    }
}