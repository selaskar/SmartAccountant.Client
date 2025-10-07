using SmartAccountant.Client.ViewModels;
using SmartAccountant.Models;

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

        var navigationParameter = new Dictionary<string, object>
        {
            { TransactionDetailsPageModel.TransactionObjectKey, transaction }
        };
        await Shell.Current.GoToAsync("/details", navigationParameter);
    }
}