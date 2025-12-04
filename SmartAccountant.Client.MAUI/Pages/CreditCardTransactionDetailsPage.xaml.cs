using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class CreditCardTransactionDetailsPage : ContentPage
{
	public CreditCardTransactionDetailsPage(CreditCardTransactionDetailsPageModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}