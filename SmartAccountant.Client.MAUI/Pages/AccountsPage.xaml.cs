using SmartAccountant.Client.ViewModels;
using SmartAccountant.Models;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class AccountsPage : ContentPage
{
    public AccountsPage(AccountsPageModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not Account account)
            return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "AccountId", account.Id }
        };
        await Shell.Current.GoToAsync("/transactions", navigationParameter);
    }
}