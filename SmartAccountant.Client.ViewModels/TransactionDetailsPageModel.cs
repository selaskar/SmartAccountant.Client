using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Models;

namespace SmartAccountant.Client.ViewModels;

public abstract partial class TransactionDetailsPageModel(INavigationService navigationService, ICoreServiceClient coreServiceClient) 
    : ViewModelBase, IQueryAttributable
{
    public const string TransactionObjectKey = "Transaction";

    /// <exception cref="ArgumentNullException"/>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        Transaction = (Transaction)query[TransactionObjectKey];

        Transaction.BeginEdit();
    }

    [ObservableProperty]
    public partial Transaction? Transaction { get; set; }

    partial void OnTransactionChanged(Transaction? oldValue, Transaction? newValue)
    {
        oldValue?.ErrorsChanged -= Transaction_ErrorsChanged;
        newValue?.ErrorsChanged += Transaction_ErrorsChanged;

        SaveCommand.NotifyCanExecuteChanged();
    }

    private void Transaction_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }


    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save(CancellationToken cancellationToken)
    {
        Transaction!.EndEdit();

        switch (Transaction)
        {
            case DebitTransaction debitTransaction:
                await coreServiceClient.UpdateDebitTransactionAsync(debitTransaction, cancellationToken);
                break;
            case CreditCardTransaction ccTransaction:
                await coreServiceClient.UpdateCreditCardTransactionAsync(ccTransaction, cancellationToken);
                break;
            default:
                break;
        }

        navigationService.NavigateBack();
    }

    private bool CanSave() => !Transaction?.HasErrors ?? false;


    [RelayCommand]
    private void Cancel()
    {
        if (Transaction?.IsEditing == true)
        {
            //TODO: ask before cancelling.
            Transaction?.CancelEdit();
        }

        navigationService.NavigateBack();
    }
}
