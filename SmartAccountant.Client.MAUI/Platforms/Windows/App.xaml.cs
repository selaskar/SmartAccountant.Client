using MAUI.MSALClient;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace SmartAccountant.Client.MAUI.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // When this handler isn't involved, the common handler gets called once, but it doesn't log or display message in non-debug mode.

            // When Handled = false, calling the common handler (in SmartAccountant.Client.MAUI.App.CurrentDomain_UnhandledException) here
            // causes it to execute twice during debug (with alternating IsTerminating values),
            // even though that handler has halt instruction.

            // When Handled = true, the common handler gets called once in debug mode.
            e.Handled = true;

            (Current.Application as MAUI.App)!.CurrentDomain_UnhandledException(sender, new System.UnhandledExceptionEventArgs(e.Exception, isTerminating: false));
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            Microsoft.Maui.Controls.Application? app = Microsoft.Maui.Controls.Application.Current;
            PlatformConfig.ParentWindow = ((MauiWinUIWindow)app!.Windows[0].Handler.PlatformView!).WindowHandle;

            // configure redirect URI for your application
            PlatformConfig.RedirectUri = "http://localhost";
        }
    }
}
