using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class SignInPage : ContentPage
{
	public SignInPage(SignInPageModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}