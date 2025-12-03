using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class DebitTransactionDetailsPage : ContentPage
{
	public DebitTransactionDetailsPage(DebitTransactionDetailsPageModel viewModel)
	{
		InitializeComponent();

		BindingContext =  viewModel;
    }
}