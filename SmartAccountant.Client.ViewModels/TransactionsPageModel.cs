using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.Models;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionsPageModel(IErrorHandler errorHandler, ICoreServiceClient serviceClient) : ViewModelBase, IQueryAttributable
{
    public const string AccountIdKey = "AccountId";
    private Guid accountId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        accountId = (Guid)query[AccountIdKey];

        if (Transactions == null)
            _ = FetchTransactions(CancellationToken.None);
    }

    [ObservableProperty]
    public partial ObservableCollection<Transaction>? Transactions { get; set; }

    [RelayCommand]
    private async Task FetchTransactions(CancellationToken cancellationToken)
    {
        IsBusy = true;

        try
        {
            Transactions = (await serviceClient.GetTransactions(accountId, cancellationToken))
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
}
