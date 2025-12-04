using SmartAccountant.Client.MAUI.Resources.Text;
using SmartAccountant.Client.Models;
using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class TransactionsPage : ContentPage
{
    public TransactionsPage(TransactionsPageModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not Transaction transaction)
            return;

        // See comments about ShellNavigationQueryParameters in AccountsPage.ListView_ItemTapped.
        var navigationParameter = new ShellNavigationQueryParameters
        {
            { TransactionDetailsPageModel.TransactionObjectKey, transaction }
        };

        switch (transaction)
        {
            case DebitTransaction:
                await Shell.Current.GoToAsync("/details-debit", navigationParameter);
                break;
            case CreditCardTransaction:
                await Shell.Current.GoToAsync("/details-creditCard", navigationParameter);
                break;
            default:
                await DisplayAlert("Unsupported transaction type", $"Transaction type ({transaction.GetType().Name}) is not supported.", MessageResources.OK);
                break;
        }
    }
}
