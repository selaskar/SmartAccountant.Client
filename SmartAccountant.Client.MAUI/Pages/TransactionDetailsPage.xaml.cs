using CommunityToolkit.Maui.Behaviors;
using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class TransactionDetailsPage : ContentPage
{
	public TransactionDetailsPage(TransactionDetailsPageModel viewModel)
	{
		InitializeComponent();

		BindingContext =  viewModel;

		TextValidationBehavior ad;
	}
}