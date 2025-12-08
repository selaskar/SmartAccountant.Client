using Android.App;
using Android.Runtime;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SmartAccountant.Client.MAUI
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [Application]
    public class MainApplication : MauiApplication
    {
        private MauiApp? MauiApplication;

        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            // Java.Lang.Thread.DefaultUncaughtExceptionHandler is never hit.
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object? sender, RaiseThrowableEventArgs e)
        {
            // When this handler is not involved, in debug and release modes (on physical device),
            // the common handler (in SmartAccountant.Client.MAUI.App.CurrentDomain_UnhandledException) works fine.
            //
            // During debug mode, this handler has no apparent effect.
            //
            // In release mode, when Handled = true, the common handler doesn't get called.
            // Logging in this method works fine.
            //
            // In release mode, when Handled = false, the common handler gets called once and works fine.
            e.Handled = false;

            //MauiApplication?.Services.GetService<Microsoft.Extensions.Logging.ILogger<MainApplication>>();
        }

        protected override MauiApp CreateMauiApp() => MauiApplication = MauiProgram.CreateMauiApp();


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            MauiApplication?.Dispose();
        }
    }
}
