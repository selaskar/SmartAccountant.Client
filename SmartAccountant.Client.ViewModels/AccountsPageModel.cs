using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.ViewModels.Services;
using SmartAccountant.Models;

namespace SmartAccountant.Client.ViewModels;

public partial class AccountsPageModel : ViewModelBase
{
    private readonly IErrorHandler errorHandler;
    private readonly ICoreServiceClient serviceClient;

    public AccountsPageModel(IErrorHandler errorHandler, ICoreServiceClient serviceClient)
    {
        this.errorHandler = errorHandler;
        this.serviceClient = serviceClient;

        _ = FetchAccounts();
    }

    [ObservableProperty]
    public partial ObservableCollection<Account>? Accounts { get; set; }

    [RelayCommand]
    private async Task FetchAccounts()
    {
        IsBusy = true;

        try
        {
            Accounts = (await serviceClient.GetAccounts())
                .OrderBy(a => a.FriendlyName)
                .ToObservable();
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

    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    [ObservableProperty]
    public override partial bool IsBusy { get; set; }

    private bool CanRefresh => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private void Refresh()
    {
        //Since changing of the value of IsRefreshing property is enough to trigger the command of RefreshView,
        //we only do that here. 
        //Otherwise, command runs twice.
        IsBusy = true;
    }
}
