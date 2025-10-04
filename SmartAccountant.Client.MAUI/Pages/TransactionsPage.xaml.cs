using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class TransactionsPage : ContentPage
{
	public TransactionsPage(TransactionsPageModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}