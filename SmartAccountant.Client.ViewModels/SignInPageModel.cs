using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.MSALClient;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels;

public partial class SignInPageModel : ObservableObject
{
    private readonly IErrorHandler errorHandler;
    private readonly ICurrentUser currentUser;

    public SignInPageModel(IErrorHandler errorHandler, ICurrentUser currentUser)
    {
        this.errorHandler = errorHandler;
        this.currentUser = currentUser;

        Initialize();
    }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    [NotifyCanExecuteChangedFor(nameof(SignOutCommand))]
    [ObservableProperty]
    public partial string? Username { get; set; }

    private async void Initialize()
    {
        IsBusy = true;

        try
        {
            Username = (await currentUser.Account)?.Username;
        }
        catch (Exception ex)
        {
            errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanSignIn() => string.IsNullOrEmpty(Username);

    [RelayCommand(CanExecute = nameof(CanSignIn))]
    private async Task SignIn()
    {
        IsBusy = true;

        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            await PublicClientSingleton.Instance.AcquireTokenSilentAsync(cts.Token);
            Username = (await currentUser.Account)?.Username;
        }
        catch (Exception ex)
        {
            errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanSignOut() => !string.IsNullOrEmpty(Username);

    [RelayCommand(CanExecute = nameof(CanSignOut))]
    private async Task SignOut()
    {
        IsBusy = true;
        try
        {
            await PublicClientSingleton.Instance.SignOutAsync();
            Username = null;
        }
        catch (Exception ex)
        {
            errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
