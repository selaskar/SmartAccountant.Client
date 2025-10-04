using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
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

        _ = Initialize();
    }

    [ObservableProperty]
    public partial ObservableCollection<Account>? Accounts { get; set; }

    private async Task Initialize()
    {
        await FetchAccounts();
    }

    [RelayCommand]
    private async Task FetchAccounts()
    {
        IsBusy = true;

        try
        {
            Accounts = [.. await serviceClient.GetAccounts()];
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
