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
            { TransactionsPageModel.AccountIdKey, account.Id }
        };
        await Shell.Current.GoToAsync("/transactions", navigationParameter);
    }
}

public class AccountDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SavingAccountTemplate { get; set; }

    public DataTemplate? CreditCardTemplate { get; set; }

    public DataTemplate? VirtualCardTemplate { get; set; }

    private static readonly DataTemplate Default = new(typeof(object));

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            SavingAccount => SavingAccountTemplate,
            CreditCard => CreditCardTemplate,
            VirtualCard => VirtualCardTemplate,
            _ => null,
        } ?? Default;
    }
}