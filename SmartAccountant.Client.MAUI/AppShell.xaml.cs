using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.MAUI;

public partial class AppShell : Shell
{
    private readonly ICurrentUser currentUser;

    public AppShell(ICurrentUser currentUser)
    {
        //Needs to be before InitializeComponent.
        this.currentUser = currentUser;

        InitializeComponent();
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (args.CanCancel && currentUser.IsAuthenticated != true)
            args.Cancel();
    }
}
