using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.ViewModels.Services;
using SmartAccountant.Models;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionsPageModel(IErrorHandler errorHandler, ICoreServiceClient serviceClient) : ViewModelBase, IQueryAttributable
{
    private Guid accountId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        accountId = (Guid)query["AccountId"];

        Refresh();
    }

    [ObservableProperty]
    public partial ObservableCollection<Transaction>? Transactions { get; set; }

    [RelayCommand]
    private async Task FetchTransactions()
    {
        IsBusy = true;

        try
        {
            Transactions = (await serviceClient.GetTransactions(accountId))
                .OrderByDescending(t => t.Timestamp)
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
