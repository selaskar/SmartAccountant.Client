using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.MSALClient;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels;

public partial class SignInPageModel : ViewModelBase
{
    private readonly IErrorHandler _errorHandler;
    private readonly ICurrentUser _currentUser;
    private readonly IAuthenticationService _authenticationService;

    public SignInPageModel(IErrorHandler errorHandler, ICurrentUser currentUser, IAuthenticationService authenticationService)
    {
        _errorHandler = errorHandler;
        _currentUser = currentUser;
        _authenticationService = authenticationService;

        _ = Initialize();
    }

    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    [NotifyCanExecuteChangedFor(nameof(SignOutCommand))]
    [ObservableProperty]
    public partial string? Username { get; set; }

    private async Task Initialize()
    {
        IsBusy = true;

        try
        {
            Username = (await _currentUser.Account)?.GetDisplayName();

            // If there is sign-in info, silently fetch access token.
            if (Username != null)
            {
                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                await SignIn(cts.Token);
            }
        }
        catch (Exception ex)
        {
            _errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanSignIn() => string.IsNullOrEmpty(Username);

    [RelayCommand(CanExecute = nameof(CanSignIn), IncludeCancelCommand = true)]
    private async Task SignIn(CancellationToken cancellationToken)
    {
        IsBusy = true;

        try
        {
            await _authenticationService.SignIn(cancellationToken);
            Username = (await _currentUser.Account)?.GetDisplayName();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _errorHandler.HandleError(ex);
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
            await _authenticationService.SignOut();

            Username = null;
        }
        catch (Exception ex)
        {
            _errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
