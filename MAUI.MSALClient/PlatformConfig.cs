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
        /// Platform specific Redirect URI
        /// </summary>
        public static string RedirectUri { get; set; }

        /// <summary>
        /// Platform specific parent window
        /// </summary>
        public static object? ParentWindow { get; set; }
    }
}
