namespace SmartAccountant.Maui.Pages;

public partial class MasterPage : ContentPage
{
    public MasterPage(MasterPageModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}