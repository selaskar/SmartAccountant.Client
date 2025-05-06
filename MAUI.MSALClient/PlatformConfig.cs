// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace MAUI.MSALClient
{
    /// <summary>
    /// Platform specific configuration.
    /// </summary>
    public static class PlatformConfig
    {
        /// <summary>
        /// Platform specific parent window
        /// </summary>
        public static object? ParentWindow { get; set; }


        private static readonly TaskCompletionSource<bool> _redirectUriSetTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>
        /// Platform specific Redirect URI
        /// </summary>
        public static string? RedirectUri
        {
            get => _redirectUri;
            set
            {
                _redirectUri = value;
                if (!string.IsNullOrEmpty(value))
                {
                    _redirectUriSetTcs.TrySetResult(true);
                }
            }
        }
        private static string? _redirectUri;

        public static Task WaitForRedirectUriAsync() => _redirectUriSetTcs.Task;
    }
}
