using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class DebitTransactionDetailsPage : ContentPage
{
	public DebitTransactionDetailsPage(DebitTransactionDetailsPageModel viewModel)
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