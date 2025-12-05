using Microsoft.Extensions.Logging;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.MAUI.Resources.Text;

namespace SmartAccountant.Client.MAUI
{
    public partial class App : Application
    {
        private readonly ILogger<App>? _logger;

        public App(ILogger<App> logger)
        {
            _logger = logger;

            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        internal void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is OperationCanceledException)
                return;

            ShowError((Exception)e.ExceptionObject);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs eventArgs)
        {
            eventArgs.SetObserved(); // Prevents crashes
            ShowError(eventArgs.Exception);
        }

        private void ShowError(Exception ex)
        {
            UnhandledExceptionOccurred(ex);

            // This will not work, if the app goes multi-windowed in the future.
            Windows[0].Page?.Dispatcher.Dispatch(() => Windows[0].Page?.DisplayAlert(MessageResources.Error, $"An unexpected error occurred: {ex}", MessageResources.OK));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var currentUser = activationState!.Context.Services.GetRequiredService<ICurrentUser>();

            return new Window(new AppShell(currentUser));
        }


        [LoggerMessage(Level = LogLevel.Error, Message = "An unhandled error occurred.")]
        private protected partial void UnhandledExceptionOccurred(Exception ex);
    }
}