using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var currentUser = activationState.Context.Services.GetRequiredService<ICurrentUser>();

            return new Window(new AppShell(currentUser));
        }
    }
}