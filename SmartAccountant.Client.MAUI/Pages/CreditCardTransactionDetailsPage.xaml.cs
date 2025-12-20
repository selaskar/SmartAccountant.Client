using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class CreditCardTransactionDetailsPage : ContentPage
{
	public CreditCardTransactionDetailsPage(CreditCardTransactionDetailsPageModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

		// See https://github.com/dotnet/maui/issues/33139
		Shell.SetBackButtonBehavior(this, new BackButtonBehavior
		{
			Command = viewModel.CancelCommand,
		});
	}
}