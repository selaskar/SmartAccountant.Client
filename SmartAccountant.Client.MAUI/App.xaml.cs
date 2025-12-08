using System.Globalization;
using System.Text;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.MAUI.Resources.Text;

namespace SmartAccountant.Client.MAUI
{
    public partial class App : Application
    {
        private static readonly CompositeFormat AppExiting = CompositeFormat.Parse(MessageResources.AppExiting);

        private protected readonly ILogger<App>? _logger;

        public App(ILogger<App> logger)
        {
            _logger = logger;

            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        internal void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is not Exception ex
                || ex is OperationCanceledException)
                return;

            UnhandledExceptionOccurred(ex);

            ShowMessage(ex);

            Thread.Sleep(10000); // Allows time for the log to be published.

            Current!.Quit();
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs eventArgs)
        {
            // Even when 'Observed' is not set to true, the app doesn't crash, and therefore publishes the error log, successfully. 
            
            eventArgs.SetObserved();

            UnhandledExceptionOccurred(eventArgs.Exception);

            ShowMessage(eventArgs.Exception);
        }


        private static void ShowMessage(Exception ex)
        {
            // In Android, cannot show message from a worker thread.
            if (OperatingSystem.IsAndroid() && !MainThread.IsMainThread)
                return;

            var toast = Toast.Make(string.Format(CultureInfo.InvariantCulture, AppExiting, ex.Message), ToastDuration.Long);
            _ = toast.Show();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var currentUser = activationState!.Context.Services.GetRequiredService<ICurrentUser>();

            return new Window(new AppShell(currentUser));
        }


        [LoggerMessage(Level = LogLevel.Critical, Message = "An unhandled error occurred.")]
        private protected partial void UnhandledExceptionOccurred(Exception ex);
    }
}