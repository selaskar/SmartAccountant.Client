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
            { DebitTransactionDetailsPageModel.TransactionObjectKey, transaction }
        };
        await Shell.Current.GoToAsync("/details", navigationParameter);
    }
}