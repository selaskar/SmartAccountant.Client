// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace SmartAccountant.Maui.Platforms.Android
{
    [Activity(Exported =true)]
    [IntentFilter([Intent.ActionView],
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal49b17825-66c1-40ad-b43f-b8d6d5e57679")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
