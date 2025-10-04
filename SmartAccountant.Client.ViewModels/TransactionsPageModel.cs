using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.ViewModels.Services;
using SmartAccountant.Models;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionsPageModel(IErrorHandler errorHandler, ICoreServiceClient serviceClient) : ViewModelBase, IQueryAttributable
{
    private Guid accountId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        accountId = (Guid)query["AccountId"];
       
        _ = Initialize();
    }

    [ObservableProperty]
    public partial ObservableCollection<Transaction>? Transactions { get; set; }

    private async Task Initialize()
    {
        await FetchTransactions();
    }

    [RelayCommand]
    private async Task FetchTransactions()
    {
        IsBusy = true;

        try
        {
            Transactions = [.. await serviceClient.GetTransactions(accountId)];
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
