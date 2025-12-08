using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.Models;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels;

public partial class AccountsPageModel : ViewModelBase
{
    private readonly IErrorHandler errorHandler;
    private readonly ICoreServiceClient serviceClient;

    public AccountsPageModel(IErrorHandler errorHandler, ICoreServiceClient serviceClient)
    {
        this.errorHandler = errorHandler;
        this.serviceClient = serviceClient;

        _ = FetchAccounts(CancellationToken.None);
    }

    [ObservableProperty]
    public partial ObservableCollection<Account>? Accounts { get; set; }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task FetchAccounts(CancellationToken cancellationToken)
    {
        IsBusy = true;

        try
        {
            Accounts = (await serviceClient.GetAccounts(cancellationToken))
                .OrderBy(a => a.FriendlyName)
                .ToObservable();
        }
        catch (CoreServiceException ex)
        {
            errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [NotifyCanExecuteChangedFor(nameof(FetchAccountsCommand))]
    [ObservableProperty]
    public override partial bool IsBusy { get; set; }

    private bool CanRefresh => !IsBusy;
}
